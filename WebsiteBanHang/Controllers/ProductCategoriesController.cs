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
    [Route("api")]
    [ApiController]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly SaleDBContext _context;
        private readonly IMapper _mapper;

        public ProductCategoriesController(SaleDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ProductCategories
        [HttpGet("admin/category/select-full")]
        public async Task<IActionResult> GetCategorySelectAll()
        {
            var category = await _context.ProductCategories.Select(p => new CategorySelectViewModel
            {
                CategoryId = p.CategoryId,
                CategoryName = p.CategoryName
            }).ToListAsync();
            return Ok(category);
        }
        [HttpGet("admin/category/select-product")]
        public async Task<IActionResult> GetCategorySelectProduct()
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
        [HttpGet("admin/category/check-url/{url}")]
        public async Task<IActionResult> CheckUrl(string url)
        {
            var category = await _context.ProductCategories.Where(p => p.Url == url).FirstOrDefaultAsync();
            if (category == null)
                return Ok(new { success = true });
            return Ok(new { success = false });
        }
        [HttpGet("category/menu")]
        public IEnumerable<Menu> GetMenu()
        {
            var allCategory = _context.ProductCategories.Include(p => p.CategoryChildrens).ThenInclude(d => d.CategoryChildrens).AsNoTracking().Where(p => p.ParentId == null).ToList();
            var menu = _mapper.Map<List<Menu>>(allCategory);
            return menu;
        }

        [HttpGet("category/{url}/{pagenumber}")]
        public async Task<IActionResult> GetProductCategoriesByUrl([FromRoute] string url, [FromRoute] int pagenumber)
        {
            int size = 1;


            var ctg = await _context.ProductCategories.Include(p => p.Products).ThenInclude(i => i.ProductImages)
                .Include(p => p.CategoryChildrens)
                    .ThenInclude(d => d.CategoryChildrens)
                        
                .Include(p => p.CategoryChildrens)
                    .ThenInclude(d => d.Products).ThenInclude(i => i.ProductImages)
                .Include(p => p.CategoryChildrens)
                    .ThenInclude(d => d.CategoryChildrens)
                        .ThenInclude(c => c.Products).ThenInclude(i => i.ProductImages)

                .Include(p => p.Products).ThenInclude(r => r.EvaluationQuestions)
                .Include(p => p.CategoryChildrens)
                    .ThenInclude(d => d.Products).ThenInclude(r => r.EvaluationQuestions)
                .Include(p => p.CategoryChildrens)
                    .ThenInclude(d => d.CategoryChildrens)
                        .ThenInclude(c => c.Products).ThenInclude(r => r.EvaluationQuestions)
                .Where(p => p.Url == url).SingleOrDefaultAsync();

            if(ctg == null)
            {
                return NotFound();
            }

            List<Products> pd = new List<Products>();
            if (ctg.Products.Count != 0)
            {
                pd.AddRange(ctg.Products);
            }
            foreach (ProductCategories categories in ctg.CategoryChildrens)
            {
                if (categories.Products.Count != 0)
                {
                    pd.AddRange(categories.Products);
                }
                foreach (ProductCategories categories1 in categories.CategoryChildrens)
                {
                    if (categories1.Products.Count != 0)
                    {
                        pd.AddRange(categories1.Products);
                    }
                }
            }

            var navbar = _context.ProductCategories.Include(p => p.CategoryChildrens).AsNoTracking().Where(p => p.Url == url).ToList();
            List<ProductCategories> categorySamelevel = new List<ProductCategories>();
            var productCategories = _context.ProductCategories.Where(p => p.Url == url).SingleOrDefault();
            if (productCategories.ParentId != null)
            {
                categorySamelevel = _context.ProductCategories.AsNoTracking().Where(p => p.ParentId == productCategories.ParentId && p.Url != url).ToList();
            }

            navbar.AddRange(categorySamelevel);
            var category_map = _mapper.Map<List<ProductCategoryViewModel>>(navbar);

            List<ProductCategories> parentCategory = new List<ProductCategories>();
            int? breadId = productCategories.CategoryId;
            do
            {
                var bread = _context.ProductCategories.Where(p => p.CategoryId == breadId).ToList();
                breadId = bread[0].ParentId;
                parentCategory.AddRange(bread);
            } while (breadId != null);
            var breadcrumb = _mapper.Map<List<Breadcrumbs>>(parentCategory);
            breadcrumb.Reverse();

            List<ProductShowcaseViewModel> productShowcase = new List<ProductShowcaseViewModel>();
            foreach (var products in pd)
            {
                if(products.Stock != 0 && !products.Discontinued)
                {
                    float star = 0;
                    int totalStar;
                    var evaluation = products.EvaluationQuestions.Where(e => e.Rate != null && e.ProductId == products.ProductId).ToList();
                    for (int i = 1; i <= 5; i++)
                    {
                        star += i * evaluation.Where(e => e.Rate == i).Count();
                    }
                    totalStar = evaluation.Count();
                    if (totalStar > 0)
                        star = star / totalStar;
                    else star = 0;
                    productShowcase.Add(new ProductShowcaseViewModel {
                        ProductId = products.ProductId,
                        Discount = products.Discount,
                        ProductName = products.ProductName,
                        UnitPrice = products.UnitPrice,
                        Rate = star,
                        TotalRate = totalStar,
                        Image = products.ProductImages.FirstOrDefault()?.Url
                    });
                } 
            }
            var product = productShowcase.Skip(size * (pagenumber - 1)).Take(size).ToList();
            int totalProducts = productShowcase.Count();
            int totalPages = (int)Math.Ceiling(totalProducts / (float)size);
            var outputModel = new CategoryOutputViewModel
            {
                Paging = new Paging(totalProducts, pagenumber, size, totalPages),
                Products = product,
                Categories = category_map,
                Breadcrumbs = breadcrumb
            };
            return Ok(outputModel);
        }

        // GET: api/ProductCategories/5
        [HttpGet("category/{id}")]
        public async Task<IActionResult> GetProductCategories([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productCategories = await _context.ProductCategories.FindAsync(id);

            if (productCategories == null)
            {
                return NotFound();
            }

            return Ok(productCategories);
        }

        // PUT: api/ProductCategories/5
        [HttpPut("category/{id}")]
        public async Task<IActionResult> PutProductCategories([FromRoute] int id, [FromBody] ProductCategories productCategories)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productCategories.CategoryId)
            {
                return BadRequest();
            }

            _context.Entry(productCategories).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductCategoriesExists(id))
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

        // POST: api/ProductCategories
        [HttpPost("admin/category")]
        public async Task<IActionResult> PostProductCategories([FromBody] ProductCategories productCategories)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProductCategories.Add(productCategories);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { id = productCategories.CategoryId });
        }

        // DELETE: api/ProductCategories/5
        [HttpDelete("category/{id}")]
        public async Task<IActionResult> DeleteProductCategories([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productCategories = await _context.ProductCategories.FindAsync(id);
            if (productCategories == null)
            {
                return NotFound();
            }

            _context.ProductCategories.Remove(productCategories);
            await _context.SaveChangesAsync();

            return Ok(productCategories);
        }

        private bool ProductCategoriesExists(int id)
        {
            return _context.ProductCategories.Any(e => e.CategoryId == id);
        }
    }
}