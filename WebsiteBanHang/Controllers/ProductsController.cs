﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebsiteBanHang.Helpers;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;

namespace WebsiteBanHang.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IHostingEnvironment _environment;
        private readonly SaleDBContext _context;
        private readonly IMapper _mapper;

        public ProductsController(SaleDBContext context, IMapper mapper, IHostingEnvironment environment)
        {
            _context = context;
            _mapper = mapper;
            _environment = environment;
        }

        [HttpGet("admin/[controller]")]
        public IEnumerable<Products> GetProducts([FromQuery] int page, [FromQuery] int size)
        {
            return _context.Products;
        }

        [HttpGet("admin/category-select")]
        public async Task<IActionResult> GetCategory()
        {
            var category = await _context.ProductCategories.Where(p => p.CategoryChildrens.Count() == 0)
                .Select(p => new
                {
                    p.CategoryId,
                    p.CategoryName
                }).AsNoTracking().ToListAsync();
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        [HttpGet("admin/[controller]/{id}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            var products = await _context.Products.Include(p => p.ProductImages).Include(p => p.OrderImportGoodsDetails).ThenInclude(o => o.Order).Where(p => p.ProductId == id).FirstOrDefaultAsync();

            if (products == null)
            {
                return NotFound();
            }
            var price = products.OrderImportGoodsDetails;
            var map_price = _mapper.Map<List<PriceImport>>(price);

            var result = new ProductPriceImportViewModel
            {
                Products = products,
                PriceImport = map_price
            };

            return Ok(result);
        }

        [HttpGet("{search}")]
        public IEnumerable<Products> SearchProducts([FromRoute] string search)
        {
            var query = new string[]
            {
                "abc foo bar xyz john doe",
                "abc foo bar xyz doe",
                "hello world",
                "abc foo bar john doe",
            };
            var abc = "abc john world";
            var searchstrings = abc.Split(' ');

            searchstrings = searchstrings.Select(x => x.ToLower()).ToArray();

            var results = query.Select(x => x.ToLower())
                               .Where(x => searchstrings.Any(y => x.Contains(y)));
            return _context.Products;
        }
        [HttpGet("admin/[controller]/search/{keyword}")]
        public async Task<IActionResult> Recommend([FromRoute] string keyword)
        {
            
            if (Int32.TryParse(keyword, out int id))
            {
                var results = await _context.Products.Where(x => x.ProductId == id).Select(x => new
                {
                    x.ProductId,
                    x.ProductName
                }).ToListAsync();
                return Ok(results);
            }
            var searchString = keyword.Split(' ');
            searchString = searchString.Select(x => x.ToLower()).ToArray();
            var rs = await _context.Products.Where(p => searchString.All(s => p.ProductName.ToLower().Contains(s))).Select(x => new
            {
                x.ProductId,
                x.ProductName
            }).ToListAsync();
            return Ok(rs);
        }

        [HttpGet("products_show")]
        public IQueryable<ProductsViewModel> GetProductsShow()//GetIndexProducts
        {
            return _context.Products.Include(p => p.ProductImages).Where(u => u.DisplayIndex == true && u.Discontinued != true && u.Stock > 0).Select(item => new ProductsViewModel
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                //Image = item.Image,
                //Rate = item.Rate,
                ProductImage = item.ProductImages
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStockProduct([FromRoute] int id)
        {
            var stock = await _context.Products.Where(p => p.ProductId == id).Select(i => new { i.Stock }).SingleOrDefaultAsync();
            return Ok(stock);
        }
        [HttpGet("[controller]/{id}")]
        public async Task<IActionResult> GetProductInformation([FromRoute] int id)
        {
            var product = await _context.Products.Include(p => p.ProductImages).Select(i => new ProductInformationViewModel
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Discount = i.Discount,
                //Image = i.Image,
                //Rate = i.Rate,
                Description = i.Description,
                Guarantee = i.Guarantee,
                Stock = i.Stock,
                Summary = i.Summary,
                ProductImage = i.ProductImages
            }).SingleOrDefaultAsync(x => x.ProductId == id);

            return Ok(product);
        }
        // GET: api/Products/5


        [HttpGet("{name}")]
        public async Task<IActionResult> GetProductByName([FromRoute] string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var products = await _context.Products.Where(p => name == null || p.ProductName.StartsWith(name) || p.ProductName.EndsWith(name) || p.ProductName.Contains(name)).Take(10).Select(i => new ProductSearchViewModel
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                //Image = i.Image
            }).ToListAsync();

            return Ok(products);
        }
        // PUT: api/Products/5
        [HttpPut("[controller]/{id}")]
        public async Task<IActionResult> PutProducts(IFormFileCollection files, [FromRoute] int id, [FromForm] string productObject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Products products = JsonConvert.DeserializeObject<Products>(productObject);

            if (id != products.ProductId)
            {
                return BadRequest();
            }
            
            _context.Entry(products).State = EntityState.Modified;

            try
            {
                List<string> imageList = new List<string>();
                imageList = await Files.UploadAsync(files, _environment.ContentRootPath);
                foreach (var image in imageList)
                {
                    var productImages = new ProductImages
                    {
                        ProductId = products.ProductId,
                        Url = image,
                        IsThumbnail = true,
                        CreateAt = DateTime.Now
                    };
                    await _context.ProductImages.AddAsync(productImages);
                }
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
        [HttpPost("[controller]")]
        public async Task<IActionResult> PostProducts(IFormFileCollection files, [FromForm] string productObject)
        {
            Products products = JsonConvert.DeserializeObject<Products>(productObject);
            products.ProductId = 0;
            products.CreateAt = DateTime.Now;
            await _context.Products.AddAsync(products);
            try
            {
                await _context.SaveChangesAsync();
                List<string> imageList = new List<string>();
                imageList = await Files.UploadAsync(files, _environment.ContentRootPath);
                foreach (var image in imageList)
                {
                    var productImages = new ProductImages
                    {
                        ProductId = products.ProductId,
                        Url = image,
                        IsThumbnail = true,
                        CreateAt = DateTime.Now
                    };
                    await _context.ProductImages.AddAsync(productImages);
                    
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }
            return Ok();
        }

        // DELETE: api/Products/5
        [HttpDelete("[controller]/{id}")]
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