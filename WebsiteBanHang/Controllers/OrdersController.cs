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
    public class OrdersController : ControllerBase
    {
        private readonly SaleDBContext _context;

        public OrdersController(SaleDBContext context)
        {
            _context = context;
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
            return _context.Orders.Where(e => e.Status == status).ToList();
        }
        [HttpPut("{id}/{status}")]
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

            var orders = await _context.Orders.Select(o=>o).Include(a => a.OrderDetails).ThenInclude(p => p.Product).Where(e => e.UserId == id).ToListAsync();

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

        // POST: api/Orders
        [HttpPost]
        public async Task<IActionResult> PostOrders([FromBody] Orders orders)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            orders.OrderId = 0;
            _context.Orders.Add(orders);
            var success = await _context.SaveChangesAsync();
            CartDetailsController cartDetails = new CartDetailsController(_context);
            if(success > 0 && orders.UserId != null)
            {
                await cartDetails.DeleteCart(orders.UserId);
            }
            
            //return CreatedAtAction("GetOrders", new { id = orders.OrderId }, orders);
            return Ok(orders);
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
}