using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebsiteBanHang.Helpers;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;

namespace WebsiteBanHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly SaleDBContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtIssuerOptions _jwtOptions;

        public UsersController(UserManager<User> userManager, IMapper mapper, SaleDBContext context, IOptions<JwtIssuerOptions> jwtOptions)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
        }

        // GET: api/Users
        //
        [Authorize]
        [HttpGet]
        public IActionResult GetUser()
        {
            var query = _context.User;//.Join(_context.UserInfo, u => u.Id, i => i.UserId, (u, i) => new { u, i });
            return Ok(query);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
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

            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new Response
            {
                Status = 200,
                Module = user
            });
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] Guid id, [FromBody] User user)
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

            if (id != user.Id)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 409,
                        Message = "Sai dữ liệu đầu vào"
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

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> PostRegister([FromBody] RegistrationViewModel model)
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
            var userIdentity = _mapper.Map<User>(model);
            var result = await _userManager.CreateAsync(userIdentity, model.Password);
            if (!result.Succeeded) return new ConflictObjectResult(Errors.AddErrorsToModelState(result, ModelState));
            await _context.UserInfo.AddAsync(new UserInfo { UserId = userIdentity.Id, FullName = model.FullName, BirthDate = DateTime.Now });
            await _context.SaveChangesAsync();
            await _userManager.AddToRoleAsync(userIdentity, "Member");

            var localUserInfo = await _context.UserInfo.FindAsync(userIdentity.Id);

            var jwt = await Tokens.GenerateJwt(userIdentity, localUserInfo?.FullName ?? "noname", _jwtOptions, _userManager);

            return Ok(new Response
            {
                Status = 201,
                Module = jwt
            });

        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
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

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new Response
            {
                Status = 204
            });
        }

        private bool UserExists(Guid id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}