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
        public IEnumerable<Products> GetProducts([FromQuery] int page, [FromQuery] int size)
        {
            return _context.Products;
        }

        [HttpGet("admin/[controller]/{id}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            var products = await _context.Products.Include(p => p.ProductImages).Include(p => p.OrderImportGoodsDetails)
                .Where(p => p.ProductId == id).FirstOrDefaultAsync();
            products.ProductImages = products.ProductImages.Where(p => p.IsThumbnail == true).ToList();

            if (products == null)
            {
                return NotFound();
            }
            var price = products.OrderImportGoodsDetails.OrderByDescending(x => x.OrderId).Take(1).FirstOrDefault();

            var result = new ProductPriceImportViewModel
            {
                Products = products,
                PriceImport = price.UnitPrice
            };

            return Ok(result);
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
            product.ProductImages = product.ProductImages.Where(p => p.IsThumbnail == true).ToList();

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
            var response = new Response
            {
                Module = product_map,
                Status = 200
            };
            return Ok(response);
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
        [HttpPut("admin/[controller]/{id}")]
        public async Task<IActionResult> PutProducts([FromForm] List<IFormFile> files, [FromRoute] int id, [FromForm] string product, [FromForm] string imageDelete)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Products products = JsonConvert.DeserializeObject<Products>(product);
            List<ImageDeleteViewModel> images = JsonConvert.DeserializeObject<List<ImageDeleteViewModel>> (imageDelete);

            if (id != products.ProductId)
            {
                return BadRequest();
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
            
            foreach( var image in images)
            {
                if (Files.Delete(image.Path, _environment.ContentRootPath))
                {
                    var ProductImages = _context.ProductImages.Find(image.ImageId);
                    if(ProductImages != null)
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
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
            if(productAdd.OrderId > 0)
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

            return StatusCode(201, new { orderId, product.ProductId });

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