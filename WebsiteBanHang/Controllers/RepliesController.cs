using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebsiteBanHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepliesController : ControllerBase
    {
        private readonly SaleDBContext _context;

        public RepliesController(SaleDBContext context)
        {
            _context = context;
        }

        // GET: api/Replies
        [HttpGet]
        public IEnumerable<Replies> GetReplies()
        {
            return _context.Replies;
        }

        // GET: api/Replies/5
        [HttpGet("{id}/{pagenumber}/{size}")]
        public async Task<IActionResult> GetReplies([FromRoute] int id, [FromRoute] int pagenumber, [FromRoute] int size)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var replies = await _context.Replies.Include(r => r.User).Include(r => r.InverseReplyByReplyNavigation)
                .Where(r => r.ProductId == id && r.ReplyByReply == null && r.IsRate == true).Skip(size * (pagenumber - 1)).Take(size)
                .Select(i => new ReplyEvaluateViewModel
                {
                    ReplyId = i.ReplyId,
                    UserId = i.UserId,
                    FullName = i.User.FullName,
                    IsRate = i.IsRate,
                    Rate = i.Rate,
                    Likes = i.Likes,
                    ProductId = i.ProductId,
                    ReplyByReply = i.ReplyByReply,
                    ReplyContent = i.ReplyContent,
                    ReplyDate = i.ReplyDate,
                    InverseReplyByReplyNavigation = i.InverseReplyByReplyNavigation
                }).OrderByDescending(x => x.ReplyId).ToListAsync();

            var totalItems = await _context.Replies.Where(r => r.ProductId == id && r.ReplyByReply == null && r.IsRate == true).CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (float)size);

            var outputModel = new EvaluationOutPutViewModel
            {
                Paging = new PagingHeader(totalItems, pagenumber, size, totalPages),
                Items = replies,
                Star = ListStar(id)
            };

            if (replies == null)
            {
                return NotFound();
            }
            return Ok(outputModel);
        }

        [HttpGet("{id}/{pagenumber}/{size}/{star}")]
        public async Task<IActionResult> GetRepliesStar([FromRoute] int id, [FromRoute] int pagenumber, [FromRoute] int size, [FromRoute] int star)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var replies = await _context.Replies.Include(r => r.User).Include(r => r.InverseReplyByReplyNavigation)
                .Where(r => r.ProductId == id && r.ReplyByReply == null && r.IsRate == true && r.Rate == star).Skip(size * (pagenumber - 1)).Take(size)
                .Select(i => new ReplyEvaluateViewModel
                {
                    ReplyId = i.ReplyId,
                    UserId = i.UserId,
                    FullName = i.User.FullName,
                    IsRate = i.IsRate,
                    Rate = i.Rate,
                    Likes = i.Likes,
                    ProductId = i.ProductId,
                    ReplyByReply = i.ReplyByReply,
                    ReplyContent = i.ReplyContent,
                    ReplyDate = i.ReplyDate,
                    InverseReplyByReplyNavigation = i.InverseReplyByReplyNavigation
                }).OrderByDescending(x => x.ReplyId).ToListAsync();

            var totalItems = await _context.Replies.Where(r => r.ProductId == id && r.ReplyByReply == null && r.IsRate == true && r.Rate == star).CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (float)size);

            var outputModel = new EvaluationOutPutViewModel
            {
                Paging = new PagingHeader(totalItems, pagenumber, size, totalPages),
                Items = replies,
                Star = ListStar(id)
            };

            if (replies == null)
            {
                return NotFound();
            }
            return Ok(outputModel);
        }
        public int[] ListStar(int id)
        {
            int[] rs = new int[5];
            for(int i= 0; i< 5; i++)
            {
                rs[i] = GetStar(id, i + 1);
            }
            return rs;
        }

        public int GetStar(int id, int star)
        {
            return _context.Replies.Where(r => r.ProductId == id && r.ReplyByReply == null && r.IsRate == true && r.Rate == star).Count();
        }
        // PUT: api/Replies/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReplies([FromRoute] int id, [FromBody] Replies replies)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != replies.ReplyId)
            {
                return BadRequest();
            }
            replies.ReplyDate = DateTime.Now;
            _context.Entry(replies).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RepliesExists(id))
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

        // POST: api/Replies
        [HttpPost]
        public async Task<IActionResult> PostReplies([FromBody] Replies replies)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            replies.ReplyId = 0;
            replies.ReplyDate = DateTime.Now;
            _context.Replies.Add(replies);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReplies", new { id = replies.ReplyId }, replies);
        }

        // DELETE: api/Replies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReplies([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var replies = await _context.Replies.FindAsync(id);
            if (replies == null)
            {
                return NotFound();
            }

            _context.Replies.Remove(replies);
            await _context.SaveChangesAsync();

            return Ok(replies);
        }

        private bool RepliesExists(int id)
        {
            return _context.Replies.Any(e => e.ReplyId == id);
        }
    }
}