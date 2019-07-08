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
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private readonly SaleDBContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IMapper _mapper;
        private readonly JwtIssuerOptions _jwtOptions;

        public UsersController(UserManager<User> userManager, IMapper mapper, SaleDBContext context, IOptions<JwtIssuerOptions> jwtOptions, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
            _roleManager = roleManager;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetUser([FromQuery] int page, [FromQuery] string keyword,[FromQuery] string role)
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
            var user = await _context.User.Include(u => u.UserInfo).ToListAsync();
            var user_map = _mapper.Map<List<UserManage>>(user);
            await Task.WhenAll(user_map.Select(async u =>
            {
                var userEntity = await _userManager.FindByIdAsync(u.UserId.ToString());
                u.Role = string.Join("; ", await _userManager.GetRolesAsync(userEntity));
            }).ToList());

            switch (role)
            {
                case "admin":
                    user_map = user_map.Where(u => u.Role == "admin").ToList();
                    break;
                case "employee":
                    user_map = user_map.Where(u => u.Role == "employee").ToList();
                    break;
                case "member":
                    user_map = user_map.Where(u => u.Role == "member").ToList();
                    break;
            }
            if (!String.IsNullOrEmpty(keyword) && keyword != "undefined")
            {
                var rs = user_map.Where(p => p.UserId.ToString() == keyword).ToList();
                if (!rs.Any())
                {
                    user_map = user_map.Where(p => p.FullName != null).ToList();
                    var searchString = keyword.Split(' ');
                    searchString = searchString.Select(x => x.ToLower()).ToArray();
                    rs = user_map.Where(p => searchString.All(s => p.FullName.ToLower().Contains(s))).ToList();
                }
                user_map = rs;
            }
            int size = 10;
            int totalUsers = user_map.Count();
            int totalPages = (int)Math.Ceiling(totalUsers / (float)size);
            page = (page < 1) ? 1 : ((page > totalPages) ? totalPages : page);
            user_map = user_map.Skip(size * (page - 1)).Take(size).ToList();
            var output = new PagingUserManage
            {
                Paging = new Paging(totalUsers, page, size, totalPages),
                UserManages = user_map
            };

            return Ok(new Response
            {
                Status = 200,
                Module = output
            });

        }

        [HttpGet("change-role/{id}/{role}")]
        public async Task<IActionResult> ChangeRole(Guid id, string role)
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
            var user = await _userManager.FindByIdAsync(id.ToString());
            if(user == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "not found user"
                });
            }
            var oldRole = await _userManager.GetRolesAsync(user);
            var exist = await _roleManager.RoleExistsAsync(role);
            if (!exist)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "not found role"
                });
            }
            if(role != oldRole[0])
            {
                var remove = await _userManager.RemoveFromRoleAsync(user, oldRole[0]);
                var add = await _userManager.AddToRoleAsync(user, role);
                if(remove.Succeeded && add.Succeeded)
                {
                    return Ok(new Response
                    {
                        Status = 204
                    });
                }
            }
            return Ok(new Response
            {
                IsError = true,
                Status = 401,
                Message = "role not change"
            });
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
        [Authorize(Roles = "member,admin,employee")]
        [HttpPut("changepassword/{id}")]
        public async Task<IActionResult> ChangePassword([FromRoute] Guid id, [FromBody] Password password)
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
                    Message = "Tài khoản không tồn tại"
                });
            }
            bool result = await _userManager.CheckPasswordAsync(user, password.PassOld);
            if (!result)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 406,
                    Message = "Sai mật khẩu"
                });
            }
            else
            {
                var rs = await _userManager.ChangePasswordAsync(user, password.PassOld, password.PassNew);
                if (rs.Succeeded)
                {
                    return Ok(new Response
                    {
                        Status = 200
                    });
                }
                else
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 409
                    });
                }
            }
        }

        // POST: api/Users
        [HttpPost]
        [AllowAnonymous]
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