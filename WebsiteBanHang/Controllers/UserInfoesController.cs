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
    [Route("api/userinfo")]
    [ApiController]
    public class UserInfoesController : ControllerBase
    {
        private readonly SaleDBContext _context;
        private readonly IMapper _mapper;

        public UserInfoesController(SaleDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/UserInfoes
        [HttpGet]
        public IActionResult GetUser()
        {
            var query = _context.UserInfo.Join(_context.User, u => u.UserId, i => i.Id,
                (u, i) => new { u.UserId, i.PhoneNumber, u.Gender, u.FullName, u.BirthDate, i.Email });
            return Ok(query);
        }

        // GET: api/UserInfoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserInfo([FromRoute] Guid id)
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

            var userInfo = await _context.UserInfo.Include(u => u.User).Where(u => u.UserId == id).SingleOrDefaultAsync();

            if (userInfo == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy thông tin người dùng"
                });
            }
            var userInfo_map = _mapper.Map<UserInfoViewModel>(userInfo);
            
            return Ok(new Response
            {
                Status = 200,
                Module = userInfo_map
            });
        }

        // PUT: api/UserInfoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserInfo([FromRoute] Guid id, [FromBody] UserInfoViewModel userInfo)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Message = "dữ liệu đầu vào sai",
                    Status = 400
                });
            }
            if (id != userInfo.UserId)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Message = "dữ liệu đầu vào sai",
                    Status = 400
                });
            }
            if (userInfo.BirthDate == null)
            {
                userInfo.BirthDate = DateTime.Now;
            }
            User user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Message = "dữ liệu chỉnh sửa không tồn tại",
                    Status = 404
                });
            }
            user.PhoneNumber = userInfo.PhoneNumber;
            UserInfo info = _mapper.Map<UserInfo>(userInfo);
            _context.Entry(user).State = EntityState.Modified;
            _context.Entry(info).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserInfoExists(id))
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Message = "dữ liệu chỉnh sửa không tồn tại",
                        Status = 404
                    });
                }
                else
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Message = "có lỗi khi chỉnh sửa",
                        Status = 409
                    });
                    throw;
                }
            }

            return Ok(new Response
            {
                Status = 204
            });
        }

        // POST: api/UserInfoes
        [HttpPost]
        public async Task<IActionResult> PostUserInfo([FromBody] UserInfo userInfo)
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

            _context.UserInfo.Add(userInfo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserInfoExists(userInfo.UserId))
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
                Status = 201,
                Module = userInfo
            });
        }

        // DELETE: api/UserInfoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserInfo([FromRoute] Guid id)
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

            var userInfo = await _context.UserInfo.FindAsync(id);
            if (userInfo == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }

            _context.UserInfo.Remove(userInfo);
            await _context.SaveChangesAsync();

            return Ok(new Response
            {
                Status = 204
            });
        }

        private bool UserInfoExists(Guid id)
        {
            return _context.UserInfo.Any(e => e.UserId == id);
        }
    }
}