using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;

namespace WebsiteBanHang.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "member,admin,employee")]
    public class OrderDetailsController : ControllerBase
    {
        private readonly SaleDBContext _context;

        public OrderDetailsController(SaleDBContext context)
        {
            _context = context;
        }


        [HttpGet("{orderId}/{productId}")]
        public async Task<IActionResult> ExistOrderDetails([FromRoute] int orderId, [FromRoute] int productId)
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
            var exits = await _context.OrderDetails.FirstOrDefaultAsync(e => e.OrderId == orderId && e.ProductId == productId);
            if (exits == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }
            return Ok(new Response
            {
                Status = 200,
                Module = exits
            });
        }
        // GET: api/OrderDetails/5
        [HttpGet("{orderId}/{productId}")]
        public async Task<IActionResult> GetOrderDetails([FromRoute] int orderId, [FromRoute] int productId)
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

            var orderDetails = await _context.OrderDetails.FirstOrDefaultAsync(e => e.OrderId == orderId && e.ProductId == productId);

            if (orderDetails == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }

            return Ok(new Response
            {
                Status = 200,
                Module = orderDetails
            });
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetailsById([FromRoute] int orderId)
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

            var orderDetails = await _context.OrderDetails.Where(e => e.OrderId == orderId).ToListAsync();

            if (orderDetails == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }

            return Ok(new Response
            {
                Status = 200,
                Module = orderDetails
            });
        }
        // PUT: api/OrderDetails/5
        [HttpPut("{orderId}/{productId}")]
        public async Task<IActionResult> PutOrderDetails([FromRoute] int orderId, [FromRoute] int productId, [FromBody] OrderDetails orderDetails)
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

            if (orderId != orderDetails.OrderId)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }

            _context.Entry(orderDetails).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderDetailsExists(orderId, productId))
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
                        Status = 400,
                        Message = "Sai dữ liệu đầu vào"
                    });
                    throw;
                }
            }

            return Ok(new Response
            {
                Status = 204
            });
        }

        // POST: api/OrderDetails
        [HttpPost]
        public async Task<IActionResult> PostOrderDetails([FromBody] OrderDetails orderDetails)
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
            if (OrderDetailsExists(orderDetails.OrderId, orderDetails.ProductId))
            {
                OrderDetails orderdetail = await _context.OrderDetails
                    .Where(e => e.OrderId == orderDetails.OrderId && e.ProductId == orderDetails.ProductId).SingleOrDefaultAsync();
                if(orderdetail != null)
                {
                    ++orderdetail.Quantity;
                    orderdetail.UnitPrice = GetPriceProduct(orderDetails.ProductId);

                }
                
            }
            _context.OrderDetails.Add(orderDetails);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OrderDetailsExists(orderDetails.OrderId,orderDetails.ProductId))
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 409,
                        Message = "Không thể lưu dữ liệu"
                    });
                }
                else
                {
                    throw;
                }
            }
            return Ok(new Response
            {
                Status = 201,
                Module = orderDetails
            });
        }

        // DELETE: api/OrderDetails/5
        [HttpDelete("{orderId}/{productId}")]
        public async Task<IActionResult> DeleteOrderDetails([FromRoute] int orderId, [FromRoute] int productId)
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

            var orderDetails = await _context.OrderDetails.FirstOrDefaultAsync(e => e.OrderId == orderId && e.ProductId == productId);
            if (orderDetails == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }

            _context.OrderDetails.Remove(orderDetails);
            await _context.SaveChangesAsync();

            return Ok(new Response
            {
                Status = 204
            });
        }

        private bool OrderDetailsExists(int orderId, int? productId)
        {
            return _context.OrderDetails.Any(e => e.OrderId == orderId && e.ProductId == productId);
        }
        private decimal GetPriceProduct(int? productId)
        {
            return (decimal)_context.Products.Where(e => e.ProductId == productId).Select(i => i.UnitPrice).FirstOrDefault();
        }
    }
}