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
    public class CartDetailsController : ControllerBase
    {
        private readonly SaleDBContext _context;

        public CartDetailsController(SaleDBContext context)
        {
            _context = context;
        }

        // GET: api/CartDetails
        [HttpGet]
        public IEnumerable<CartDetails> GetCartDetails()
        {
            return _context.CartDetails;
        }

        // GET: api/CartDetails/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartDetails([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cartDetails = await _context.CartDetails.Join(_context.Products,
              c => c.ProductId,
              p => p.ProductId,
              (c, p) => new
              {
                 c.UserId, c.ProductId, c.Quantity,
                 p.ProductName, p.UnitPrice, /*p.Image,*/ p.Stock, p.Discount, p.Summary
              }).Where(e => e.UserId == id).ToListAsync();

            if (!cartDetails.Any())
            {
                return NotFound();
            }

            return Ok(cartDetails);
        }

        // PUT: api/CartDetails/5
        [HttpPut("{userId}/{productId}")]
        public async Task<IActionResult> PutCartDetails([FromRoute] Guid userId, [FromRoute] int productId, [FromBody] CartDetails cartDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userId != cartDetails.UserId)
            {
                return BadRequest();
            }

            _context.Entry(cartDetails).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartDetailsExists(userId, productId))
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
        // POST: api/CartDetails
        [HttpPost]
        public async Task<IActionResult> PostCartDetails([FromBody] CartDetails cartDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(CartDetailsExists(cartDetails.UserId,cartDetails.ProductId))
            {
                CartDetails cart = await _context.CartDetails
                    .Where(e => e.ProductId == cartDetails.ProductId && e.UserId == cartDetails.UserId).SingleOrDefaultAsync();
                if(cart != null)
                {
                    cart.Quantity++;
                    return await PutCartDetails(cart.UserId,cart.ProductId,cart);
                }
            }
            cartDetails.Quantity = 1;
            _context.CartDetails.Add(cartDetails);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CartDetailsExists(cartDetails.UserId,cartDetails.ProductId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCartDetails", new { id = cartDetails.UserId }, cartDetails);
        }

        // DELETE: api/CartDetails/5
        [HttpDelete("{userId}/{productId}")]
        public async Task<IActionResult> DeleteCartDetails([FromRoute] Guid userId, [FromRoute] int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cartDetails = await _context.CartDetails.Where(e => e.ProductId == productId && e.UserId == userId).SingleOrDefaultAsync();
            if (cartDetails == null)
            {
                return NotFound();
            }

            _context.CartDetails.Remove(cartDetails);
            await _context.SaveChangesAsync();

            return Ok(cartDetails);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteCart([FromRoute] Guid userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cartDetails = await _context.CartDetails.Where(e => e.UserId == userId).ToListAsync();
            if (cartDetails == null)
            {
                return NotFound();
            }

            _context.CartDetails.RemoveRange(cartDetails);
            await _context.SaveChangesAsync();

            return Ok(cartDetails);
        }
        private bool CartDetailsExists(Guid id, int productId)
        {
            return _context.CartDetails.Any(e => e.UserId == id && e.ProductId == productId);
        }
    }
}