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
                CategoryName = p.CategoryName,
                isLast = false
            }).ToListAsync();

            var listChildren = await _context.ProductCategories.Where(p => p.CategoryChildrens.Count() == 0)
                .Select(p => p.CategoryId).ToListAsync();

            category.ForEach(e =>
            {
                if (listChildren.Contains(e.CategoryId))
                {
                    e.isLast = true;
                }
            });

            if (!category.Any())
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "không tìm thấy dữ liệu"
                });
            }
            return Ok(new Response
            {
                Module = category,
                Status = 200
            });
        }

        //[HttpGet("admin/category/select-full")]
        //public async Task<IActionResult> GetCategorySelectAll()
        //{
        //    var category = await _context.ProductCategories.Select(p => new CategorySelectViewModel
        //    {
        //        CategoryId = p.CategoryId,
        //        CategoryName = p.CategoryName
        //    }).ToListAsync();
        //    if (!category.Any())
        //    {
        //        return Ok(new Response
        //        {
        //            IsError = true,
        //            Status = 404,
        //            Message = "không tìm thấy dữ liệu"
        //        });
        //    }
        //    return Ok(new Response
        //    {
        //        Module = category,
        //        Status = 200
        //    });
        //}
        [HttpGet("admin/category/select-product")]
        public async Task<IActionResult> GetCategorySelectProduct()
        {
            var category = await _context.ProductCategories.Where(p => p.CategoryChildrens.Count() == 0)
                .Select(p => new
                {
                    p.CategoryId,
                    p.CategoryName
                }).AsNoTracking().ToListAsync();

            if (!category.Any())
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "không tìm thấy dữ liệu"
                });

            return Ok(new Response
            {
                Status = 200,
                Module = category
            });
        }
        [HttpGet("admin/category/check-url/{url}")]
        public async Task<IActionResult> CheckUrl(string url)
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
            var category = await _context.ProductCategories.Where(p => p.Url == url).FirstOrDefaultAsync();
            if (category == null)
                return Ok(new Response
                {
                    Status = 204
                });
            return Ok(new Response
            {
                Status = 400
            });
        }
        [HttpGet("menu/category")]
        public async Task<IActionResult> GetMenu()
        {
            var allCategory = await _context.ProductCategories.Include(p => p.CategoryChildrens).ThenInclude(d => d.CategoryChildrens).AsNoTracking().Where(p => p.ParentId == null).ToListAsync();
            if (!allCategory.Any())
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "không tìm thấy dữ liệu"
                });
            }
            var menu = _mapper.Map<List<Menu>>(allCategory);
            return Ok(new Response
            {
                Status = 200,
                Module = menu
            });
        }

        [HttpGet("category/{url}")]
        public async Task<IActionResult> GetProductCategoriesByUrl([FromRoute] string url, [FromQuery] int page, [FromQuery] string order)
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
            int size = 10;

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
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "không tìm thấy dữ liệu"
                });
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
            var category_map = new ProductCategoryViewModel();
            if(ctg.ParentId != null)
            {
                var categoryParent = await _context.ProductCategories.Where(p => p.CategoryId == ctg.ParentId).Select(p => new ProductCategories
                {
                    CategoryName = p.CategoryName,
                    Url = p.Url,
                    CategoryId = p.CategoryId,
                    CategoryChildrens = p.CategoryChildrens.Select(q => new ProductCategories
                    {
                        CategoryId = q.CategoryId,
                        CategoryName = q.CategoryName,
                        Url = q.Url,
                        CategoryChildrens = q.CategoryChildrens.Where(a => a.ParentId == ctg.CategoryId).ToList()
                    }).OrderByDescending(m => m.CategoryId == ctg.CategoryId).ToList()
                }).FirstOrDefaultAsync();

                category_map = _mapper.Map<ProductCategoryViewModel>(categoryParent);
            }
            else
            {
                var categoryNoParent = await _context.ProductCategories.Include(p => p.CategoryChildrens).AsNoTracking().SingleOrDefaultAsync(p => p.CategoryId == ctg.CategoryId);
                category_map = _mapper.Map<ProductCategoryViewModel>(categoryNoParent);
            }

            List<ProductCategories> parentCategory = new List<ProductCategories>();
            int? breadId = ctg.CategoryId;
            do
            {
                var bread = _context.ProductCategories.Where(p => p.CategoryId == breadId).SingleOrDefault();
                breadId = bread.ParentId;
                parentCategory.Add(bread);
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
            switch (order)
            {
                case "newest":
                    productShowcase = productShowcase.OrderByDescending(p => p.ProductId).ToList();
                    break;
                case "discount":
                    productShowcase = productShowcase.OrderByDescending(p => p.Discount).ToList();
                    break;
                case "priceasc":
                    productShowcase = productShowcase.OrderBy(p => p.UnitPrice).ToList();
                    break;
                case "pricedesc":
                    productShowcase = productShowcase.OrderByDescending(p => p.UnitPrice).ToList();
                    break;
            }
            int totalProducts = productShowcase.Count();
            int totalPages = (int)Math.Ceiling(totalProducts / (float)size);
            page = (page < 1) ? 1 : ((page > totalPages) ? totalPages : page);
            var product = productShowcase.Skip(size * (page - 1)).Take(size).ToList();
            
            var outputModel = new CategoryOutputViewModel
            {
                Paging = new Paging(totalProducts, page, size, totalPages),
                Products = product,
                Categories = category_map,
                Breadcrumbs = breadcrumb
            };
            return Ok(new Response
            {
                Status = 200,
                Module = outputModel
            });
        }

        // GET: api/ProductCategories/5
        //[HttpGet("category/{id}")]
        //public async Task<IActionResult> GetProductCategories([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var productCategories = await _context.ProductCategories.FindAsync(id);

        //    if (productCategories == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(productCategories);
        //}

        // PUT: api/ProductCategories/5
        [HttpPut("category/{id}")]
        public async Task<IActionResult> PutProductCategories([FromRoute] int id, [FromBody] ProductCategories productCategories)
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

            if (id != productCategories.CategoryId)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
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

        // POST: api/ProductCategories
        [HttpPost("admin/category")]
        public async Task<IActionResult> PostProductCategories([FromBody] ProductCategories productCategories)
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

            _context.ProductCategories.Add(productCategories);
            await _context.SaveChangesAsync();
            return Ok(new Response
            {
                Status = 201,
                Module = productCategories.CategoryId
            });
            return StatusCode(201, new { id = productCategories.CategoryId });
        }

        // DELETE: api/ProductCategories/5
        [HttpDelete("category/{id}")]
        public async Task<IActionResult> DeleteProductCategories([FromRoute] int id)
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

            var productCategories = await _context.ProductCategories.FindAsync(id);
            if (productCategories == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }

            _context.ProductCategories.Remove(productCategories);
            await _context.SaveChangesAsync();
            return Ok(new Response
            {
                Status = 204
            });
        }

        private bool ProductCategoriesExists(int id)
        {
            return _context.ProductCategories.Any(e => e.CategoryId == id);
        }
    }
}