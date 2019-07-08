using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Helpers;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;

namespace WebsiteBanHang.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(Roles = "member,admin,employee")]
    public class OrdersController : ControllerBase
    {
        private readonly SaleDBContext _context;
        private readonly IMapper _mapper;

        public OrdersController(SaleDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [Authorize(Roles = "admin,employee")]
        [HttpGet("orders/update/{id}/{status}")]
        public async Task<IActionResult> PutConfirmOrders([FromRoute] int id, [FromRoute] int status)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }

            Orders orders = _context.Orders.Include(o => o.OrderDetails).Where(o => o.OrderId == id).SingleOrDefault();

            orders.Status = status;
            if(status == Globals.KHACH_HUY || status == Globals.SHOP_HUY)
            {
                foreach(var details in orders.OrderDetails)
                {
                    var product = _context.Products.Find(details.ProductId);
                    product.Stock += details.Quantity;
                    _context.Entry(product).State = EntityState.Modified;
                }
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
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 404,
                        Message = "Không tìm thấy dữ liệu"
                    });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new Response
            {
                Status = 204
            });
        }
        [Authorize(Roles = "member,admin,employee")]
        [HttpGet("orders/cancel/{userid}/{orderid}")]
        public async Task<IActionResult> CancelOrderUser([FromRoute] int orderid, [FromRoute] Guid userid)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }

            Orders orders = _context.Orders.Include(o => o.OrderDetails).Where(o => o.OrderId == orderid).SingleOrDefault();
            if(orders.Status != Globals.CHO_XAC_NHAN)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 409,
                    Message = "Dữ liệu đã thay đổi"
                });
            }
            if(orders.UserId != userid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 403,
                    Message = "Bạn không có quyền"
                });
            }
            orders.Status = Globals.KHACH_HUY;
            foreach (var details in orders.OrderDetails)
            {
                var product = _context.Products.Find(details.ProductId);
                product.Stock += details.Quantity;
                _context.Entry(product).State = EntityState.Modified;
            }
            _context.Entry(orders).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdersExists(orderid))
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 404,
                        Message = "Không tìm thấy dữ liệu"
                    });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new Response
            {
                Status = 204
            });
        }
        // GET: api/Orders/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetOrderByIdOrder([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Ok(new Response
        //        {
        //            IsError = true,
        //            Status = 400,
        //            Message = "Sai dữ liệu đầu vào"
        //        });
        //    }

        //    var orders = await _context.Orders.FindAsync(id);

        //    if (orders == null)
        //    {
        //        return Ok(new Response
        //        {
        //            IsError = true,
        //            Status = 404,
        //            Message = "Không tìm thấy dữ liệu"
        //        });
        //    }
        //    orders.OrderDetails = await _context.OrderDetails.Where(e => e.OrderId == id).ToListAsync();
        //    return Ok(new Response
        //    {
        //        Status = 200,
        //        Module = orders
        //    });
        //}
        [Authorize(Roles = "member,admin,employee")]
        [HttpGet("user/orders/{id}")]
        public async Task<IActionResult> GetOrderByIdUser([FromRoute] Guid id, [FromQuery] int status, [FromQuery] int page)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }
            int size = 5;
            var order = await _context.Orders.Include(p => p.OrderDetails).ThenInclude(od => od.Product)
                .ThenInclude(pr => pr.ProductImages).Include(p => p.Wards).ThenInclude(w => w.Districts)
                .ThenInclude(d => d.Provinces).Include(p => p.OrderStatus).Include(p => p.User)
                .AsNoTracking().Where(p => p.UserId == id).ToListAsync();

            var order_map = _mapper.Map<List<OrdersViewModel>>(order);
            if (status != 0 && status != Globals.KHACH_HUY && status != Globals.SHOP_HUY)
            {
                order_map = order_map.Where(o => o.Status == status).ToList();
            }
            if (status == Globals.KHACH_HUY || status == Globals.SHOP_HUY)
            {
                order_map = order_map.Where(o => o.Status == Globals.SHOP_HUY || o.Status == Globals.KHACH_HUY).ToList();
            }
            order_map = order_map.OrderByDescending(o => o.OrderId).ToList();
            int totalOrders = order_map.Count();
            int totalPages = (int)Math.Ceiling(totalOrders / (float)size);
            page = (page < 1) ? 1 : ((page > totalPages) ? totalPages : page);
            var order_page = order_map.Skip(size * (page - 1)).Take(size).ToList();

            var outputModel = new OrderOutputViewModel
            {
                Paging = new Paging(totalOrders, page, size, totalPages),
                Orders = order_page
            };
            return Ok(new Response
            {
                Status = 200,
                Module = outputModel
            });
        }

        [Authorize(Roles = "admin,employee")]
        [HttpGet("admin/orders/check-history/{id}")]
        public async Task<IActionResult> CheckHistotryOrder(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }
            var order = await _context.Orders.Where(p => p.UserId == id).ToListAsync();
            int success = order.Where(p => p.Status == Globals.DA_GIAO).Count();
            int fail = order.Where(p => p.Status == Globals.KHACH_HUY).Count();
            int totalOrder = success + fail;
            int percent = 0;
            if(totalOrder > 0)
            {
                percent = (success / totalOrder)*100;
            }

            var rate = await _context.EvaluationQuestions.Where(p => p.UserId == id && p.Rate != null).ToListAsync();
            float star = 0;
            int[] starList = new int[5];
            for (int i = 1; i <= 5; i++)
            {
                int temp = rate.Where(e => e.Rate == i).Count();
                starList[i - 1] = temp;
                star += i * temp;
            }
            int totalStar = rate.Count();
            if (totalStar > 0)
                star = (float)Math.Round((double)star / totalStar, 1);
            else star = 0;
            var output = new HistoryBuy
            {
                Percent = percent,
                Rate = star,
                TotalOrder = totalOrder,
                TotalStar = totalStar
            };
            return Ok(new Response
            {
                Status = 200,
                Module = output
            });

        }

        [Authorize(Roles = "admin,employee")]
        [HttpGet("admin/orders")]
        public async Task<IActionResult> GetOrderByStatus([FromQuery] int status, [FromQuery] int page,[FromQuery] int id,[FromQuery] int size,[FromQuery] string sort)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }
            var order = await _context.Orders.Include(p => p.OrderDetails).ThenInclude(od => od.Product).ThenInclude(pr => pr.ProductImages)
                .Include(p => p.Wards).ThenInclude(w => w.Districts).ThenInclude(d => d.Provinces).Include(p => p.OrderStatus).Include(p => p.User).ToListAsync();

            int[] countOrder = new int[6];
            countOrder[0] = order.Count();
            for(int i = 1;i <= 4; i++)
            {
                countOrder[i] = order.Where(p => p.Status == i).Count();
            }
            countOrder[5] = order.Where(p => p.Status == 5 || p.Status == 6).Count();

            var order_map = _mapper.Map<List<OrdersViewModel>>(order);
            if(status != 0 && status != Globals.KHACH_HUY && status != Globals.SHOP_HUY)
            {
                order_map = order_map.Where(o => o.Status == status).ToList();
            }
            if(id != 0)
            {
                order_map = order_map.Where(o => o.OrderId == id).ToList();
            }
            if(status == Globals.KHACH_HUY || status == Globals.SHOP_HUY)
            {
                order_map = order_map.Where(o => o.Status == Globals.SHOP_HUY || o.Status == Globals.KHACH_HUY).ToList();
            }
            switch (sort)
            {
                case "datedesc":
                    order_map = order_map.OrderByDescending(p => p.OrderId).ToList();
                    break;
                case "pricedesc":
                    order_map = order_map.OrderByDescending(p => p.TotalPrice).ToList();
                    break;
                case "priceasc":
                    order_map = order_map.OrderBy(p => p.TotalPrice).ToList();
                    break;
            }
            if(size < 1)
            {
                size = 10;
            }
            int totalOrders = order_map.Count();
            int totalPages = (int)Math.Ceiling(totalOrders / (float)size);
            page = (page < 1) ? 1 : ((page > totalPages) ? totalPages : page);
            var order_page = order_map.Skip(size * (page - 1)).Take(size).ToList();

            var outputModel = new OrderOutputViewModel
            {
                Paging = new Paging(totalOrders, page, size, totalPages),
                Orders = order_page,
                CountOrder = countOrder
            };
            return Ok(new Response
            {
                Status = 200,
                Module = outputModel
            });
        }

        // PUT: api/Orders/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutOrders([FromRoute] int id, [FromBody] Orders orders)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Ok(new Response
        //        {
        //            IsError = true,
        //            Status = 400,
        //            Message = "Sai dữ liệu đầu vào"
        //        });
        //    }

        //    if (id != orders.OrderId)
        //    {
        //        return Ok(new Response
        //        {
        //            IsError = true,
        //            Status = 400,
        //            Message = "Sai dữ liệu đầu vào"
        //        });
        //    }

        //    _context.Entry(orders).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!OrdersExists(id))
        //        {
        //            return Ok(new Response
        //            {
        //                IsError = true,
        //                Status = 404,
        //                Message = "Không tìm thấy dữ liệu"
        //            });
        //        }
        //        else
        //        {
        //            return Ok(new Response
        //            {
        //                IsError = true,
        //                Status = 400,
        //                Message = "Không thể lưu dữ liệu"
        //            });
        //            throw;
        //        }
        //    }

        //    return Ok(new Response
        //    {
        //        Status = 204
        //    });
        //}
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
        [Authorize(Roles = "member,admin,employee")]
        [HttpPost("orders/{addressId}")]
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
                    Status = 418
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
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteOrders([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Ok(new Response
        //        {
        //            IsError = true,
        //            Status = 400,
        //            Message = "Sai dữ liệu đầu vào"
        //        });
        //    }

        //    var orders = await _context.Orders.FindAsync(id);
        //    if (orders == null)
        //    {
        //        return Ok(new Response
        //        {
        //            IsError = true,
        //            Status = 404,
        //            Message = "Không tìm thấy dữ liệu"
        //        });
        //    }

        //    _context.Orders.Remove(orders);
        //    await _context.SaveChangesAsync();

        //    return Ok(new Response
        //    {
        //        Status = 204
        //    });
        //}

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
            if (x == null) return false;
            if (y == null) return false;
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