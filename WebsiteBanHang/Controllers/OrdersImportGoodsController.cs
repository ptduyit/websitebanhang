using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;

namespace WebsiteBanHang.Controllers
{
    [Route("api/admin")]
    [ApiController]
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
        [HttpGet]
        public IEnumerable<OrdersImportGoods> GetOrdersImportGoods()
        {
            return _context.OrdersImportGoods;
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

            var ordersImportGoods = await _context.OrdersImportGoods.Include(o => o.User).Include(o => o.Supplier).Include(o => o.OrderImportGoodsDetails)
                .ThenInclude(i => i.Product).Where(o => o.OrderId == id).FirstOrDefaultAsync();

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
            foreach(var detail in ordersView.Product)
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
        public async Task<IActionResult> CreateOrderDetail([FromBody] OrderImportDetailsViewModel detail )
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
        [HttpDelete("{id}")]
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
            if(orderDetail == null)
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