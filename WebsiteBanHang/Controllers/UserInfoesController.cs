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
    [Route("api/[controller]")]
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
                return BadRequest(ModelState);
            }

            var userInfo = await _context.UserInfo.Include(u => u.User).Where(u => u.UserId == id)
                .Select(u => new
                {
                    u.UserId,
                    u.Gender,
                    u.BirthDate,
                    u.User.PhoneNumber,
                    u.FullName
                }).SingleOrDefaultAsync();

            if (userInfo == null)
            {
                return NotFound();
            }

            return Ok(userInfo);
        }

        // PUT: api/UserInfoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserInfo([FromRoute] Guid id, [FromBody] UserInfoViewModel userInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != userInfo.UserId)
            {
                return BadRequest();
            }
            if (userInfo.BirthDate != null)
            {
                TimeSpan time = new TimeSpan(7, 0, 0);
                userInfo.BirthDate = userInfo.BirthDate.Add(time);
            }
            User user = await _context.User.FindAsync(id);
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // POST: api/UserInfoes
        [HttpPost]
        public async Task<IActionResult> PostUserInfo([FromBody] UserInfo userInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUserInfo", new { id = userInfo.UserId }, userInfo);
        }

        // DELETE: api/UserInfoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserInfo([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userInfo = await _context.UserInfo.FindAsync(id);
            if (userInfo == null)
            {
                return NotFound();
            }

            _context.UserInfo.Remove(userInfo);
            await _context.SaveChangesAsync();

            return Ok(userInfo);
        }

        private bool UserInfoExists(Guid id)
        {
            return _context.UserInfo.Any(e => e.UserId == id);
        }
    }
}