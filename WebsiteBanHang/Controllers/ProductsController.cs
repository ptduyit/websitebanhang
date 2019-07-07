using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebsiteBanHang.Helpers;
using WebsiteBanHang.Hubs;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;

namespace WebsiteBanHang.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private static bool flag = false;
        private readonly IHostingEnvironment _environment;
        private readonly SaleDBContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<EchoHub> _hubContext;

        public ProductsController(SaleDBContext context, IMapper mapper, IHostingEnvironment environment, IHubContext<EchoHub> hubContext)
        {
            _context = context;
            _mapper = mapper;
            _environment = environment;
            _hubContext = hubContext;
        }

        [HttpGet("admin/[controller]")]
        public async Task<IActionResult> GetProducts([FromQuery] int page, [FromQuery] int size, [FromQuery] string status, [FromQuery] string keyword, [FromQuery] string categoryid)
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
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            var products_map = _mapper.Map<List<ProductManage>>(products);
            if (Int32.TryParse(categoryid, out int cid))
            {
                if (cid > 0)
                {
                    products_map = products_map.Where(p => p.CategoryId == cid).ToList();
                }
            }
            switch (status)
            {
                case "stock":
                    products_map = products_map.Where(p => p.Stock < 6).ToList();
                    break;
                case "discontinued":
                    products_map = products_map.Where(p => p.Discontinued == true).ToList();
                    break;
                case "index":
                    products_map = products_map.Where(p => p.DisplayIndex == true).ToList();
                    break;
                case "price":
                    products_map = products_map.Where(p => p.UnitPrice == 0).ToList();
                    break;
            }
            if (!String.IsNullOrEmpty(keyword) && keyword != "undefined")
            {
                if (Int32.TryParse(keyword, out int id))
                {
                    products_map = products_map.Where(p => p.ProductId == id).ToList();
                }
                else
                {
                    products_map = products_map.Where(p => p.ProductName != null).ToList();
                    var searchString = keyword.Split(' ');
                    searchString = searchString.Select(x => x.ToLower()).ToArray();
                    products_map = products_map.Where(p => searchString.All(s => p.ProductName.ToLower().Contains(s))).ToList();
                }
            }
            if (size < 1)
            {
                size = 10;
            }
            int totalProduct = products_map.Count();
            int totalPages = (int)Math.Ceiling(totalProduct / (float)size);
            page = (page < 1) ? 1 : ((page > totalPages) ? totalPages : page);
            var product_page = products_map.Skip(size * (page - 1)).Take(size).ToList();
            var output = new OutputProductManage
            {
                Paging = new Paging(totalProduct, page, size, totalPages),
                Products = product_page
            };
            return Ok(new Response
            {
                Status = 200,
                Module = output
            });

        }
        [HttpGet("admin/change-status/{id}/{status}")]
        public async Task<IActionResult> ChangeStatus(int id, string status)
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
            if (status == "discontinued" || status == "index")
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Message = "not found",
                        Status = 404
                    });
                }
                if (status == "discontinued")
                {
                    product.Discontinued = !product.Discontinued;
                }
                else
                {
                    product.DisplayIndex = !product.DisplayIndex;
                }
                _context.Entry(product).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Message = "not save",
                        Status = 409
                    });
                }
                return Ok(new Response
                {
                    Status = 204
                });
            }
            return Ok(new Response
            {
                IsError = true,
                Status = 400,
                Message = "Sai dữ liệu đầu vào"
            });
        }

        [HttpGet("admin/[controller]/{id}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
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
            var products = await _context.Products.Include(p => p.ProductImages).Include(p => p.OrderImportGoodsDetails)
                .Where(p => p.ProductId == id).FirstOrDefaultAsync();
            products.ProductImages = products.ProductImages.Where(p => p.IsThumbnail == true).ToList();

            if (products == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }
            var price = products.OrderImportGoodsDetails.OrderByDescending(x => x.OrderId).Take(1).FirstOrDefault();

            var result = new ProductPriceImportViewModel
            {
                Products = products,
                PriceImport = price.UnitPrice
            };

            return Ok(new Response
            {
                Status = 200,
                Module = result
            });
        }

        [HttpGet("admin/[controller]/search/{keyword}")]
        public async Task<IActionResult> Recommend([FromRoute] string keyword)
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

        [HttpGet("products/products-show")]
        public async Task<IActionResult> GetProductsShow()//GetIndexProducts
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
            var products = await _context.Products.Include(p => p.ProductImages).Include(p => p.EvaluationQuestions)
                .Where(p => p.Discontinued == false && p.Stock > 0 && p.DisplayIndex == true).ToListAsync();
            List<ProductShowcaseViewModel> productShowcase = new List<ProductShowcaseViewModel>();
            foreach (var product in products)
            {
                float star = 0;
                int totalStar;
                var evaluation = product.EvaluationQuestions.Where(e => e.Rate != null && e.ProductId == product.ProductId).ToList();
                for (int i = 1; i <= 5; i++)
                {
                    star += i * evaluation.Where(e => e.Rate == i).Count();
                }
                totalStar = evaluation.Count();
                if (totalStar > 0)
                    star = star / totalStar;
                else star = 0;
                productShowcase.Add(new ProductShowcaseViewModel
                {
                    ProductId = product.ProductId,
                    Discount = product.Discount,
                    ProductName = product.ProductName,
                    UnitPrice = product.UnitPrice,
                    Rate = star,
                    TotalRate = totalStar,
                    Image = product.ProductImages.FirstOrDefault()?.Url
                });
            }
            return Ok(new Response
            {
                Status = 200,
                Module = productShowcase
            });
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetStockProduct([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Ok(new Response
        //        {
        //            IsError = true,
        //            Status = 400,
        //            Message = "Sai dữ liệu đầu vào"
        //        });
        //    }
        //    var stock = await _context.Products.Where(p => p.ProductId == id).Select(i => new { i.Stock }).SingleOrDefaultAsync();
        //    return Ok(new Response
        //    {
        //        Status = 200,
        //        Module = stock
        //    });
        //}
        [HttpGet("[controller]/{id}")]
        public async Task<IActionResult> GetProductInformation([FromRoute] int id)
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
            if (!flag)
            {
                flag = true;
                NotifyChange n = new NotifyChange(_hubContext);
                var listId = _context.Products.Select(p => p.ProductId).ToList();
                listId.ForEach(i =>
                {
                    n.GetStock(i);
                });
            }
            var product = await _context.Products.Include(p => p.ProductImages).Include(p => p.EvaluationQuestions).SingleOrDefaultAsync(x => x.ProductId == id);
            //product.ProductImages = product.ProductImages.Where(p => p.IsThumbnail == true).ToList();
            if(product == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Sản phẩm không tồn tại"
                });
            }
            var product_map = _mapper.Map<ProductInformationViewModel>(product);
            float star = 0;
            var evaluation = product.EvaluationQuestions.Where(e => e.Rate != null && e.ProductId == product.ProductId).ToList();
            for (int i = 1; i <= 5; i++)
            {
                star += i * evaluation.Where(e => e.Rate == i).Count();
            }
            int totalStar = evaluation.Count();
            if (totalStar > 0)
                star = (float)Math.Round((double)star / totalStar, 1);
            else star = 0;

            product_map.NumRate = totalStar;
            product_map.Rate = star;
            return Ok(new Response
            {
                Module = product_map,
                Status = 200
            });

        }
        // GET: api/Products/5


        [HttpGet("[controller]/quick-search")]
        public async Task<IActionResult> GetProductByName([FromQuery] string keyword)
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
            if(String.IsNullOrEmpty(keyword) || keyword == "undefined")
            {
                return Ok(new Response
                {
                    Status = 200,
                    Module = new List<int>()
                });
            }
            var searchString = keyword.Split(' ');
            searchString = searchString.Select(x => x.ToLower()).ToArray();
            var rs = await _context.Products.Include(p => p.ProductImages).Where(p => searchString.All(s => p.ProductName.ToLower().Contains(s)) && p.Discontinued == false && p.Stock > 0).Select(x => new
            {
                x.ProductId,
                x.ProductName,
                x.UnitPrice,
                Image = x.ProductImages.FirstOrDefault(p => p.IsThumbnail == true).Url
            }).Take(5).ToListAsync();
            return Ok(new Response
            {
                Status = 200,
                Module = rs
            });
        }
        [HttpGet("[controller]/search")]
        public async Task<IActionResult> SearchProduct([FromQuery] string keyword, [FromQuery] int page, [FromQuery] string sort)
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
            if (String.IsNullOrEmpty(keyword) || keyword == "undefined")
            {
                return Ok(new Response
                {
                    Status = 200,
                    Module = new List<int>()
                });
            }
            int size = 10;
            var searchString = keyword.Split(' ');
            searchString = searchString.Select(x => x.ToLower()).ToArray();
            var productsAll = await _context.Products.Include(p => p.ProductImages).Include(p => p.EvaluationQuestions)
                .Where(p => searchString.All(s => p.ProductName.ToLower().Contains(s)) && p.Discontinued == false && p.Stock > 0).ToListAsync();
            var productsAny = await _context.Products.Include(p => p.ProductImages).Include(p => p.EvaluationQuestions)
                .Where(p => searchString.Count(s => p.ProductName.ToLower().Contains(s)) > 1 && p.Discontinued == false && p.Stock > 0).ToListAsync();
            productsAll.AddRange(productsAny);
            var products = productsAll.GroupBy(x => x.ProductId).Select(y => y.First()).ToList();
            List<ProductShowcaseViewModel> productShowcase = new List<ProductShowcaseViewModel>();
            foreach (var product in products)
            {
                float star = 0;
                int totalStar;
                var evaluation = product.EvaluationQuestions.Where(e => e.Rate != null && e.ProductId == product.ProductId).ToList();
                for (int i = 1; i <= 5; i++)
                {
                    star += i * evaluation.Where(e => e.Rate == i).Count();
                }
                totalStar = evaluation.Count();
                if (totalStar > 0)
                    star = star / totalStar;
                else star = 0;
                productShowcase.Add(new ProductShowcaseViewModel
                {
                    ProductId = product.ProductId,
                    Discount = product.Discount,
                    ProductName = product.ProductName,
                    UnitPrice = product.UnitPrice,
                    Rate = star,
                    TotalRate = totalStar,
                    Image = product.ProductImages.FirstOrDefault()?.Url
                });
            }
            switch (sort)
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
            var product_page = productShowcase.Skip(size * (page - 1)).Take(size).ToList();
            var output = new
            {
                Paging = new Paging(totalProducts, page, size, totalPages),
                Products = product_page
            };
            return Ok(new Response
            {
                Status = 200,
                Module = output
            });
        }
        // PUT: api/Products/5
        [HttpPut("admin/[controller]/{id}")]
        public async Task<IActionResult> PutProducts([FromForm] List<IFormFile> files, [FromRoute] int id, [FromForm] string product, [FromForm] string imageDelete)
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

            Products products = JsonConvert.DeserializeObject<Products>(product);
            List<ImageDeleteViewModel> images = JsonConvert.DeserializeObject<List<ImageDeleteViewModel>>(imageDelete);

            if (id != products.ProductId)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }

            _context.Entry(products).State = EntityState.Modified; //modify product

            var imageList = await Files.UploadAsync(files, _environment.ContentRootPath); //upload image
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

            foreach (var image in images)
            {
                if (Files.Delete(image.Path, _environment.ContentRootPath))
                {
                    var ProductImages = _context.ProductImages.Find(image.ImageId);
                    if (ProductImages != null)
                        _context.ProductImages.Remove(ProductImages);
                }
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductsExists(id))
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
        [HttpPost("admin/[controller]/quick-add")]
        public async Task<IActionResult> QuickAddProduct(QuickAddProductViewModel productAdd)
        {
            int orderId = productAdd.OrderId;
            var product = new Products
            {
                ProductName = productAdd.ProductName,
                CategoryId = productAdd.CategoryId,
                CreateAt = DateTime.Now,
                Description = "",
                Discontinued = true,
                DisplayIndex = false,
                Discount = 0,
                Stock = 0,
                UnitPrice = 0,
                Summary = "",
                Guarantee = 0
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            if (productAdd.OrderId > 0)
            {
                var orderDetail = new OrderImportGoodsDetails
                {
                    OrderId = productAdd.OrderId,
                    ProductId = product.ProductId,
                    Quantity = productAdd.Quantity,
                    UnitPrice = productAdd.UnitPrice
                };
                _context.OrderImportGoodsDetails.Add(orderDetail);
                await _context.SaveChangesAsync();
            }
            else
            {
                OrdersImportGoods orders = new OrdersImportGoods
                {
                    OrderDate = DateTime.Now,
                    SupplierId = productAdd.SupplierId,
                    UserId = productAdd.UserId,
                    TotalPrice = 0
                };
                orders.OrderImportGoodsDetails.Add(new OrderImportGoodsDetails
                {
                    ProductId = product.ProductId,
                    Quantity = productAdd.Quantity,
                    UnitPrice = productAdd.UnitPrice
                });
                _context.OrdersImportGoods.Add(orders);
                await _context.SaveChangesAsync();
                orderId = orders.OrderId;
            }
            return Ok(new Response
            {
                Status = 201,
                Module = new { orderId, product.ProductId }
            });
            return StatusCode(201, new { orderId, product.ProductId });

        }

        // DELETE: api/Products/5
        [HttpDelete("[controller]/{id}")]
        public async Task<IActionResult> DeleteProducts([FromRoute] int id)
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

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }

            _context.Products.Remove(products);
            await _context.SaveChangesAsync();

            return Ok(new Response
            {
                Status = 204
            });
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}