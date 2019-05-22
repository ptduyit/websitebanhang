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
                return BadRequest(ModelState);
            }

            var ordersImportGoods = await _context.OrdersImportGoods.Include(o => o.User).Include(o => o.Supplier).Include(o => o.OrderImportGoodsDetails)
                .ThenInclude(i => i.Product).Where(o => o.OrderId == id).FirstOrDefaultAsync();

            if (ordersImportGoods == null)
            {
                return NotFound();
            }
            var order_map = _mapper.Map<OrderImportViewModel>(ordersImportGoods);

            return Ok(order_map);
        }

        // PUT: api/OrdersImportGoods/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrdersImportGoods([FromRoute] int id, [FromBody] OrdersImportGoods ordersImportGoods)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ordersImportGoods.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(ordersImportGoods).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrdersImportGoodsExists(id))
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

        // POST: api/OrdersImportGoods
        [HttpPost("order-import")]
        public async Task<IActionResult> CreateOrdersImportGoods([FromBody] OrderImportFirstViewModel order)
        {
            OrdersImportGoods orders = new OrdersImportGoods
            {
                OrderDate = DateTime.Now,
                SupplierId = order.SupplierId,
                UserId = order.UserId
            };
            orders.OrderImportGoodsDetails.Add(new OrderImportGoodsDetails
            {
                ProductId = order.ProductId,
                Quantity = 0,
                UnitPrice = 0
            });

            _context.OrdersImportGoods.Add(orders);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction("GetOrdersImportGoods", new { id = orders.OrderId });
        }
        [HttpPost("orderdetail-import")]
        public async Task<IActionResult> CreateOrderDetail([FromBody] OrderImportDetailsViewModel detail )
        {
            OrderImportGoodsDetails orderdetail = new OrderImportGoodsDetails
            {
                OrderId = detail.OrderId,
                ProductId = detail.ProductId,
                UnitPrice = 0,
                Quantity = 0
            };
            _context.OrderImportGoodsDetails.Add(orderdetail);
            await _context.SaveChangesAsync();
            return StatusCode(201);
        }

        // DELETE: api/OrdersImportGoods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdersImportGoods([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ordersImportGoods = await _context.OrdersImportGoods.FindAsync(id);
            if (ordersImportGoods == null)
            {
                return NotFound();
            }

            _context.OrdersImportGoods.Remove(ordersImportGoods);
            await _context.SaveChangesAsync();

            return Ok(ordersImportGoods);
        }
        [HttpDelete("order-import-detail/oid/{oid}/pid/{pid}")]
        public async Task<IActionResult> DeleteOrderDetai([FromRoute] int oid, [FromRoute] int pid)
        {
            var orderDetail = await _context.OrderImportGoodsDetails.Where(p => p.OrderId == oid && p.ProductId == pid).FirstOrDefaultAsync();
            if(orderDetail == null)
            {
                return NotFound();
            }
            _context.OrderImportGoodsDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return Ok();
        }
        
        private bool OrdersImportGoodsExists(int id)
        {
            return _context.OrdersImportGoods.Any(e => e.OrderId == id);
        }
    }
}