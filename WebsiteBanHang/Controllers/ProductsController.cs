using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;

namespace WebsiteBanHang.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly SaleDBContext _context;

        public ProductsController(SaleDBContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public IEnumerable<Products> GetAllProducts()
        {
            return _context.Products;
        }

        [HttpGet]
        public IQueryable<ViewModelProduct> GetIndexProducts()
        {
            return _context.Products.Include(p => p.ProductImage).Where(u => u.DisplayIndex == true && u.Discontinued != true).Select(item => new ViewModelProduct
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                Image = item.Image,
                Rate = item.Rate,
                ProductImage = item.ProductImage
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductInformation([FromRoute] int id)
        {
            var product = await _context.Products.Include(p => p.ProductImage).Select(i => new ProductInformationViewModel
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Discount = i.Discount,
                Image = i.Image,
                Rate = i.Rate,
                Description = i.Description,
                Guarantee = i.Guarantee,
                Stock = i.Stock,
                Summary = i.Summary,
                ProductImage = i.ProductImage
            }).SingleOrDefaultAsync(x => x.ProductId == id);

            return Ok(product);
        }
        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var products = await _context.Products.FindAsync(id);

            if (products == null)
            {
                return NotFound();
            }

            return Ok(products);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducts([FromRoute] int id, [FromBody] Products products)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != products.ProductId)
            {
                return BadRequest();
            }
            products.DateUpdated = DateTime.Now;
            _context.Entry(products).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductsExists(id))
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

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> PostProducts([FromBody] Products products)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            products.ProductId = 0;
            products.DateUpdated = DateTime.Now;
            _context.Products.Add(products);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductById", new { id = products.ProductId }, products);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducts([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }

            _context.Products.Remove(products);
            await _context.SaveChangesAsync();

            return Ok(products);
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}