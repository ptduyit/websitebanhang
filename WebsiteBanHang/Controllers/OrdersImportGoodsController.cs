using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;

namespace WebsiteBanHang.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "admin,employee")]
    public class OrdersImportGoodsController : ControllerBase
    {
        private readonly SaleDBContext _context;
        private readonly IMapper _mapper;

        public OrdersImportGoodsController(SaleDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/OrdersImportGoods
        [HttpGet("order-import")]
        public async Task<IActionResult> GetOrdersImportGoods([FromQuery] string type, [FromQuery] string keyword, [FromQuery] bool temporary,[FromQuery] int page)
        {
            int size = 10;
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }
            var order = await _context.OrdersImportGoods.Include(p => p.Supplier).Include(p => p.User).OrderByDescending(p => p.OrderId).ToListAsync();
            var order_map = _mapper.Map<List<OrderImportAllViewModel>>(order);
            if (temporary)
            {
                order_map = order_map.Where(p => p.Complete == false).ToList();
            }
            if (!String.IsNullOrEmpty(keyword) && keyword != "undefined")
            {
                if (type == "user")
                {
                    var rs = order_map.Where(p => p.UserId.ToString() == keyword).ToList();
                    if (!rs.Any())
                    {
                        order_map = order_map.Where(p => p.FullName != null).ToList();
                        var searchString = keyword.Split(' ');
                        searchString = searchString.Select(x => x.ToLower()).ToArray();
                        rs = order_map.Where(p => searchString.All(s => p.FullName.ToLower().Contains(s))).ToList();
                    }
                    order_map = rs;
                }
                else if (type == "supplier")
                {
                    if (Int32.TryParse(keyword, out int id))
                    {
                        order_map = order_map.Where(p => p.SupplierId == id).ToList();
                    }
                    else
                    {
                        order_map = order_map.Where(p => p.CompanyName != null).ToList();
                        var searchString = keyword.Split(' ');
                        searchString = searchString.Select(x => x.ToLower()).ToArray();
                        order_map = order_map.Where(p => searchString.All(s => p.CompanyName.ToLower().Contains(s))).ToList();
                    }
                    
                }
                else if (type == "order")
                {
                    if (Int32.TryParse(keyword, out int id))
                    {
                        order_map = order_map.Where(p => p.OrderId == id).ToList();
                    }
                    else
                    {
                        order_map = new List<OrderImportAllViewModel>();
                    }
                }
            }
            int totalOrders = order_map.Count();
            int totalPages = (int)Math.Ceiling(totalOrders / (float)size);
            page = (page < 1) ? 1 : ((page > totalPages) ? totalPages : page);
            var order_page = order_map.Skip(size * (page - 1)).Take(size).ToList();
            var output = new OrderImportOutputViewModel
            {
                Paging = new Paging(totalOrders, page, size, totalPages),
                Orders = order_map
            };

            return Ok(new Response
            {
                Status = 200,
                Module = output
            });
        }

        // GET: api/OrdersImportGoods/5
        [HttpGet("order-import/{id}")]
        public async Task<IActionResult> GetOrdersImportGoods([FromRoute] int id)
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

            var ordersImportGoods = await _context.OrdersImportGoods.Include(o => o.User).Include(o => o.Supplier)
                .Include(o => o.OrderImportGoodsDetails).ThenInclude(i => i.Product).ThenInclude(p => p.ProductImages)                
                .Where(o => o.OrderId == id).FirstOrDefaultAsync();

            if (ordersImportGoods == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }
            var order_map = _mapper.Map<OrderImportViewModel>(ordersImportGoods);

            return Ok(new Response
            {
                Status = 200,
                Module = order_map
            });
        }

        // PUT: api/OrdersImportGoods/5
        [HttpPut("order-import/{id}/save")]
        public async Task<IActionResult> SaveOrdersImportGoods([FromRoute] int id, [FromBody] OrderImportUpdateViewModel ordersView)
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

            if (id != ordersView.OrderId)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }
            var orders = await _context.OrdersImportGoods.FindAsync(id);
            orders.SupplierId = ordersView.SupplierId;

            decimal totalPrice = 0;
            foreach (var detail in ordersView.Product)
            {
                totalPrice += detail.Quantity * detail.UnitPrice;
                detail.OrderId = id;
                _context.Entry(detail).State = EntityState.Modified;
                //update stock
                var product = await _context.Products.FindAsync(detail.ProductId);
                product.Stock += detail.Quantity;
                _context.Entry(product).State = EntityState.Modified;
            }
            orders.TotalPrice = totalPrice;
            _context.Entry(orders).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdersImportGoodsExists(id))
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
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 409,
                        Message = "Không thể lưu dữ liệu"
                    });
                    throw;
                }
            }

            return Ok(new Response
            {
                Status = 204
            });
        }
        [HttpPut("order-import/{id}/temp")]
        public async Task<IActionResult> TempOrdersImportGoods([FromRoute] int id, [FromBody] OrderImportUpdateViewModel ordersView)
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

            if (id != ordersView.OrderId)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }
            var orders = await _context.OrdersImportGoods.FindAsync(id);
            orders.SupplierId = ordersView.SupplierId;
            orders.TotalPrice = 0;
            _context.Entry(orders).State = EntityState.Modified;

            foreach (var detail in ordersView.Product)
            {
                detail.OrderId = id;
                _context.Entry(detail).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdersImportGoodsExists(id))
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
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 409,
                        Message = "Không thể lưu dữ liệu"
                    });
                    throw;
                }
            }

            return Ok(new Response
            {
                Status = 204
            });
        }
        // POST: api/OrdersImportGoods
        [HttpPost("order-import")]
        public async Task<IActionResult> CreateOrdersImportGoods([FromBody] OrderImportFirstViewModel order)
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
            OrdersImportGoods orders = new OrdersImportGoods
            {
                OrderDate = DateTime.Now,
                SupplierId = order.SupplierId,
                UserId = order.UserId,
                TotalPrice = 0
            };
            orders.OrderImportGoodsDetails.Add(new OrderImportGoodsDetails
            {
                ProductId = order.ProductId,
                Quantity = 0,
                UnitPrice = 0
            });

            _context.OrdersImportGoods.Add(orders);
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
                Status = 201,
                Module = orders.OrderId
            });
        }
        [HttpPost("orderdetail-import")]
        public async Task<IActionResult> CreateOrderDetail([FromBody] OrderImportDetailsViewModel detail)
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
            OrderImportGoodsDetails orderdetail = new OrderImportGoodsDetails
            {
                OrderId = detail.OrderId,
                ProductId = detail.ProductId,
                UnitPrice = 0,
                Quantity = 0
            };
            _context.OrderImportGoodsDetails.Add(orderdetail);
            await _context.SaveChangesAsync();
            return Ok(new Response
            {
                Status = 204
            });
        }

        // DELETE: api/OrdersImportGoods/5
        [HttpDelete("order-import/{id}")]
        public async Task<IActionResult> DeleteOrdersImportGoods([FromRoute] int id)
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

            var ordersImportGoods = await _context.OrdersImportGoods.FindAsync(id);
            if (ordersImportGoods == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }
            if(ordersImportGoods.TotalPrice > 0)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 409,
                    Message = "Không thể xóa đơn hàng này"
                });
            }
            _context.OrdersImportGoods.Remove(ordersImportGoods);
            await _context.SaveChangesAsync();
            return Ok(new Response
            {
                Status = 204
            });

        }
        [HttpDelete("order-import-detail/oid/{oid}/pid/{pid}")]
        public async Task<IActionResult> DeleteOrderDetai([FromRoute] int oid, [FromRoute] int pid)
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
            var orderDetail = await _context.OrderImportGoodsDetails.Where(p => p.OrderId == oid && p.ProductId == pid).FirstOrDefaultAsync();
            if (orderDetail == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }
            _context.OrderImportGoodsDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();

            return Ok(new Response
            {
                Status = 204
            });
        }

        private bool OrdersImportGoodsExists(int id)
        {
            return _context.OrdersImportGoods.Any(e => e.OrderId == id);
        }
    }
}