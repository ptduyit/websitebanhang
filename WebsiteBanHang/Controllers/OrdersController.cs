using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Helpers;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;

namespace WebsiteBanHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly SaleDBContext _context;
        private readonly IMapper _mapper;

        public OrdersController(SaleDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Orders
        [HttpGet]
        public IEnumerable<Orders> GetAllOrders()
        {
            return _context.Orders;
        }
        [HttpGet("{status}")]
        public IEnumerable<Orders> GetConfirmOrders([FromRoute] int status)
        {
            return _context.Orders.Include(s => s.OrderStatus).Include(a => a.OrderDetails).ThenInclude(p => p.Product).Where(e => e.Status == status).ToList();
        }
        [HttpGet("{id}/{status}")]
        public async Task<IActionResult> PutConfirmOrders([FromRoute] int id, [FromRoute] int status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Orders orders = _context.Orders.Where(o => o.OrderId == id).SingleOrDefault();
            orders.Status = status;
            _context.Entry(orders).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderByIdOrder([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orders = await _context.Orders.FindAsync(id);

            if (orders == null)
            {
                return NotFound();
            }
            orders.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == id).ToListAsync();

            return Ok(orders);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderByIdUser([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orders = await _context.Orders.Select(o => o).Include(s => s.OrderStatus).Include(a => a.OrderDetails).ThenInclude(p => p.Product).Where(e => e.UserId == id).OrderByDescending(d => d.OrderDate).ToListAsync();

            if (orders == null)
            {
                return NotFound();
            }
            //orders.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == id).ToListAsync();

            return Ok(orders);
        }
        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrders([FromRoute] int id, [FromBody] Orders orders)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != orders.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(orders).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        public bool ListEquals(List<CartOrderViewModel> list1, List<CartOrderViewModel> list2)
        {
            var cnt = new Dictionary<CartOrderViewModel, int>(new CartEqualityComparer());
            foreach (CartOrderViewModel s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (CartOrderViewModel s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }

        // POST: api/Orders
        [HttpPost("{addressId}")]
        public async Task<IActionResult> PostOrders([FromBody] List<CartOrderViewModel> cartClient, int addressId)
        {

            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Message = "Sai dữ liệu đầu vào",
                    Status = 400
                });
            }
            var userId = cartClient[0].UserId;
            var address = await _context.Address.Where(a => a.UserId == userId && a.AddressId == addressId).FirstOrDefaultAsync();
            if (address == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Message = "Sai địa chỉ nhận hàng",
                    Status = 404
                });
            }

            var cartServer = await _context.CartDetails.Include(c => c.Product).Where(c => c.UserId == userId).ToListAsync();
            if (!cartServer.Any())
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Giỏ hàng rỗng"
                });
            }
            var cart_map = _mapper.Map<List<CartOrderViewModel>>(cartServer);
            //!cart_map.OrderBy(i => i).SequenceEqual(cart_map.OrderBy(i => i))
            if (!ListEquals(cart_map,cartClient))
            {
                var cnt = new Dictionary<CartOrderViewModel, int>(new CartEqualityComparer());
                return Ok(new Response
                {
                    IsError = true,
                    Status = 409,
                    Message = "Giỏ hàng không đồng bộ"
                });
            }
            var order = new Orders
            {
                FullName = address.FullName,
                OrderDate = DateTime.Now,
                Status = Globals.CHO_XAC_NHAN,
                PhoneNumber = address.PhoneNumber,
                ShippedDate = DateTime.Now.AddDays(7),
                Street = address.Street,
                UserId = userId,
                WardId = address.WardId
            };
            decimal totalPrice = 0;
            foreach (var item in cart_map)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                product.Stock -= item.Quantity;
                if (product.Stock < 0)
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 406,
                        Message = "sản phẩm hết hàng"
                    });
                }
                _context.Entry(product).State = EntityState.Modified;
                totalPrice += item.UnitPrice * item.Quantity;
                var detail = new OrderDetails
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };
                order.OrderDetails.Add(detail);
                var cartdetail = await _context.CartDetails.Where(c => c.ProductId == item.ProductId && c.UserId == userId).SingleOrDefaultAsync();
                _context.Remove(cartdetail);
            }
            order.TotalPrice = totalPrice;
            _context.Orders.Add(order);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 409,
                    Message = "không thể lưu dữ liệu"
                });
            }
            return Ok(new Response
            {
                Status = 201
            });
            //decimal total = 0;
            //foreach(var item in orders.OrderDetails.Select((value, i) => new { i, value}))
            //{
            //    var product = await _context.Products.FindAsync(item.value.ProductId);
            //    var quantity = orders.OrderDetails[item.i].Quantity;
            //    var unitprice = product.UnitPrice;
            //    orders.OrderDetails[item.i].UnitPrice = unitprice;
            //    total += quantity * unitprice;
            //    product.Stock -= quantity;
            //    if(product.Stock < 0)
            //    {
            //        return BadRequest();
            //    }
            //    _context.Entry(product).State = EntityState.Modified;
            //}
            //var address = _context.Address.Find(addressId);
            //orders.TotalPrice = total;
            //orders.PhoneNumber = address.PhoneNumber;
            //orders.FullName = address.FullName;
            ////orders.Province = address.Province;
            ////orders.District = address.District;
            ////orders.Ward = address.Ward;
            //orders.Street = address.Street;
            //orders.OrderDate = DateTime.Now;
            //orders.ShippedDate = orders.OrderDate.AddDays(7);
            //orders.OrderId = 0;
            //orders.Status = 1;
            //_context.Orders.Add(orders);
            //var success = await _context.SaveChangesAsync();
            ////CartDetailsController cartDetails = new CartDetailsController(_context);
            //if(success > 0 && orders.UserId != null)
            //{
            //    /////await cartDetails.DeleteCart(orders.UserId);
            //}

            ////return CreatedAtAction("GetOrders", new { id = orders.OrderId }, orders);
            //return Ok(orders);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrders([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orders = await _context.Orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(orders);
            await _context.SaveChangesAsync();

            return Ok(orders);
        }

        private bool OrdersExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }

    }
    public sealed class CartEqualityComparer : IEqualityComparer<CartOrderViewModel>
    {
        public bool Equals(CartOrderViewModel x, CartOrderViewModel y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return Equals(x.UserId, y.UserId) && x.UnitPrice == y.UnitPrice && x.Quantity == y.Quantity
                && x.ProductId == y.ProductId && x.Discontinued == y.Discontinued;
        }

        public int GetHashCode(CartOrderViewModel obj)
        {
            return obj.Discontinued.GetHashCode() ^ obj.ProductId.GetHashCode() ^ obj.Quantity.GetHashCode()
                ^ obj.UnitPrice.GetHashCode() ^ obj.UserId.GetHashCode();
        }
    }
}