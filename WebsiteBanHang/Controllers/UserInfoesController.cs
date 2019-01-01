using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoesController : ControllerBase
    {
        private readonly SaleDBContext _context;

        public UserInfoesController(SaleDBContext context)
        {
            _context = context;
        }

        // GET: api/UserInfoes
        [HttpGet]
        public IActionResult GetUser()
        {
            var query = _context.UserInfo.Join(_context.User, u => u.UserId, i => i.Id, 
                (u, i) => new { u.UserId, u.Phone,u.Gender,u.FullName,u.BirthDate,i.Email });
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

            var userInfo = await _context.UserInfo.FindAsync(id);

            if (userInfo == null)
            {
                return NotFound();
            }

            return Ok(userInfo);
        }

        // PUT: api/UserInfoes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserInfo([FromRoute] Guid id, [FromBody] UserInfo userInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userInfo.UserId)
            {
                return BadRequest();
            }
            if(userInfo.BirthDate != null)
            {
                TimeSpan time = new TimeSpan(7, 0, 0);
                userInfo.BirthDate = userInfo.BirthDate.Add(time);
            }
            _context.Entry(userInfo).State = EntityState.Modified;

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

            return NoContent();
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