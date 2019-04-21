using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly SaleDBContext _context;

        public OrderDetailsController(SaleDBContext context)
        {
            _context = context;
        }

        // GET: api/OrderDetails
        [HttpGet]
        public IEnumerable<OrderDetails> GetOrderDetails()
        {
            return _context.OrderDetails;
        }
        [HttpGet("{orderId}/{productId}")]
        public async Task<IActionResult> ExistOrderDetails([FromRoute] int orderId, [FromRoute] int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exits = await _context.OrderDetails.FirstOrDefaultAsync(e => e.OrderId == orderId && e.ProductId == productId);
            if (exits == null)
            {
                return NotFound();
            }
            return StatusCode(200);
        }
        // GET: api/OrderDetails/5
        [HttpGet("{orderId}/{productId}")]
        public async Task<IActionResult> GetOrderDetails([FromRoute] int orderId, [FromRoute] int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderDetails = await _context.OrderDetails.FirstOrDefaultAsync(e => e.OrderId == orderId && e.ProductId == productId);

            if (orderDetails == null)
            {
                return NotFound();
            }

            return Ok(orderDetails);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetailsById([FromRoute] int orderId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderDetails = await _context.OrderDetails.Where(e => e.OrderId == orderId).ToListAsync();

            if (orderDetails == null)
            {
                return NotFound();
            }

            return Ok(orderDetails);
        }
        // PUT: api/OrderDetails/5
        [HttpPut("{orderId}/{productId}")]
        public async Task<IActionResult> PutOrderDetails([FromRoute] int orderId, [FromRoute] int productId, [FromBody] OrderDetails orderDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (orderId != orderDetails.OrderId)
            {
                return BadRequest();
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/OrderDetails
        [HttpPost]
        public async Task<IActionResult> PostOrderDetails([FromBody] OrderDetails orderDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOrderDetails", new { id = orderDetails.OrderId }, orderDetails);
        }

        // DELETE: api/OrderDetails/5
        [HttpDelete("{orderId}/{productId}")]
        public async Task<IActionResult> DeleteOrderDetails([FromRoute] int orderId, [FromRoute] int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderDetails = await _context.OrderDetails.FirstOrDefaultAsync(e => e.OrderId == orderId && e.ProductId == productId);
            if (orderDetails == null)
            {
                return NotFound();
            }

            _context.OrderDetails.Remove(orderDetails);
            await _context.SaveChangesAsync();

            return Ok(orderDetails);
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