using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;
using AutoMapper;
namespace WebsiteBanHang.Controllers
{
    [Route("api")]
    [ApiController]
    public class CartDetailsController : ControllerBase
    {
        private readonly SaleDBContext _context;
        private readonly IMapper _mapper;
        public CartDetailsController(SaleDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/CartDetails/5
        [HttpGet("cart/{id}")]
        public async Task<IActionResult> GetCartDetails([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Dữ liệu vào sai"
                });
            }
            var cart = await _context.CartDetails.Include(c => c.Product).ThenInclude(p => p.ProductImages).Where(c => c.UserId == id).ToListAsync();
            
            if (!cart.Any())
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Giỏ hàng rỗng"
                });
            }
            var cart_map = _mapper.Map<List<CartViewModel>>(cart);

            return Ok(new Response
            {
                Module = cart_map,
                Status = 200
            });
        }
        [HttpGet("cart/{userId}/quantity")]
        public async Task<IActionResult> GetTotalQuantity(Guid userId)
        {
            var item = await _context.CartDetails.Where(e => e.UserId == userId).Select(e => e.Quantity).ToListAsync();
            int total = 0;
            item.ForEach(e =>
            {
                total += e;
            });
            return Ok(new Response
            {
                Status = 200,
                Module = total
            });
        }

        [HttpPatch("cart/{userId}/{productId}")]
        public async Task<IActionResult> UpdateStock([FromRoute] Guid userId, [FromRoute] int productId, [FromBody] JsonPatchDocument<CartDetails> cart)
        {
            var cartdetail = await _context.CartDetails.Where(e => e.ProductId == productId && e.UserId == userId).SingleOrDefaultAsync();
            if(cartdetail == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "không tìm thấy sản phẩm để thay đổi số lượng"
                });
            }
            Task<int> t = GetStock(productId);
            await t;
            int stock = t.Result;
            var element = cart.Operations.Where(c => c.path.Equals("/quantity")).Select(c => c.value).SingleOrDefault();
            if(!Int32.TryParse(element.ToString(),out int quantity))
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 412,
                    Message = "dữ liệu đầu vào sai"
                });
            }
            if(quantity > stock)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 406,
                    Message = "Không đủ số lượng"
                });
            }
            cart.ApplyTo(cartdetail, ModelState);
            await _context.SaveChangesAsync();
            return Ok(new Response
            {
                Status = 204
            });
        }

        // POST: api/CartDetails
        [HttpPost("cart")]
        public async Task<IActionResult> PostCartDetails([FromBody] CartDetails cartDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Task<int> t = GetStock(cartDetails.ProductId);
            await t;
            int stock = t.Result;

            if (CartDetailsExists(cartDetails.UserId,cartDetails.ProductId))
            {
                var cart = await _context.CartDetails
                    .Where(e => e.ProductId == cartDetails.ProductId && e.UserId == cartDetails.UserId).SingleOrDefaultAsync();
                if(cart != null)
                {
                    if((cartDetails.Quantity+cart.Quantity) > stock)
                    {
                        return Ok(new Response
                        {
                            Status = 406,
                            IsError = true,
                            Message = "Không đủ số lượng"
                        });
                    }
                    else
                    {
                        cart.Quantity += cartDetails.Quantity;
                        _context.Entry(cart).State = EntityState.Modified;
                        try
                        {
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!CartDetailsExists(cart.UserId, cart.ProductId))
                            {
                                return Ok(new Response
                                {
                                    IsError = true,
                                    Status = 404
                                });
                            }
                            else
                            {
                                throw;
                            }
                        }
                        return Ok(new Response
                        {
                            Status = 204
                        });
                    }
                }
            }
            if (cartDetails.Quantity > stock)
            {
                return Ok(new Response
                {
                    Status = 406,
                    IsError = true,
                    Message = "Không đủ số lượng"
                });
            }
            _context.CartDetails.Add(cartDetails);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CartDetailsExists(cartDetails.UserId,cartDetails.ProductId))
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 409
                    });
                }
                else
                {
                    throw;
                }
            }
            return Ok(new Response
            {
                Status = 201,
                Module = cartDetails
            });
        }

        // DELETE: api/CartDetails/5
        [HttpDelete("cart/{userId}/{productId}")]
        public async Task<IActionResult> DeleteCartDetails([FromRoute] Guid userId, [FromRoute] int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cartDetails = await _context.CartDetails.Where(e => e.ProductId == productId && e.UserId == userId).SingleOrDefaultAsync();
            if (cartDetails == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404
                });
            }

            _context.CartDetails.Remove(cartDetails);
            await _context.SaveChangesAsync();

            return Ok(new Response
            {
                Status = 204
            });
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
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404
                });
            }

            _context.CartDetails.RemoveRange(cartDetails);
            await _context.SaveChangesAsync();

            return Ok(new Response
            {
                Status = 204
            });
        }
        private bool CartDetailsExists(Guid id, int productId)
        {
            return _context.CartDetails.Any(e => e.UserId == id && e.ProductId == productId);
        }
        private Task<int> GetStock(int productId)
        {
            return _context.Products.Where(p => p.ProductId == productId).Select(p => p.Stock).SingleOrDefaultAsync();
        }
    }
}