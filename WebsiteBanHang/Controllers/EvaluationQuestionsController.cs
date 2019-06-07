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
    public class EvaluationQuestionsController : ControllerBase
    {
        private readonly SaleDBContext _context;
        private readonly IMapper _mapper;

        public EvaluationQuestionsController(SaleDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        [HttpGet("evaluations")]
        public async Task<IActionResult> GetEvaluation([FromQuery] int productid, [FromQuery] int pagenumber)
        {
            int size = 3;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var evaluations = await _context.EvaluationQuestions.Include(e => e.User)
                .Include(e => e.Comments).ThenInclude(c => c.User)
                .Where(e => e.ProductId == productid && e.Rate != null)
                .ToListAsync();
            
            if (evaluations == null)
            {
                return NotFound();
            }
            var eval_map = _mapper.Map<List<EvaluationQuestionsViewModel>>(evaluations);

            //paging
            int totalEval = eval_map.Count();
            int totalPages = (int)Math.Ceiling(totalEval / (float)size);
            pagenumber = (pagenumber < 1) ? 1 : ((pagenumber > totalPages) ? totalPages : pagenumber);//range page
            var eval = eval_map.Skip(size * (pagenumber - 1)).Take(size).ToList();

            //get rating
            float star = 0;
            int[] starList = new int[5];
            for (int i = 1; i <= 5; i++)
            {
                int temp = eval_map.Where(e => e.Rate == i).Count();
                starList[i - 1] = temp;
                star += i * temp;
            }
            int totalStar = eval_map.Count();
            if (totalStar > 0)
                star = (float)Math.Round((double)star / totalStar,1);
            else star = 0;

            var output = new EvaluationOutputViewModel
            {
                Paging = new Paging(totalEval, pagenumber, size, totalPages),
                Rating = new Rating(star, totalStar, starList),
                Evaluations = eval
            };
            return Ok(output);
        }

        [HttpGet("{productid}/{pagenumber}")]
        public async Task<IActionResult> GetEQuestions([FromRoute] int productid, [FromRoute] int pagenumber)
        {
            int size = 1;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var evaluationQuestions = await _context.EvaluationQuestions.Include(e => e.User).Include(e => e.Comments).ThenInclude(c => c.User).Where(e => e.ProductId == productid && e.Rate == null).ToListAsync();


            if (evaluationQuestions == null)
            {
                return NotFound();
            }
            var question_map = _mapper.Map<List<EvaluationQuestionsViewModel>>(evaluationQuestions);
            var question = question_map.Skip(size * (pagenumber - 1)).Take(size).ToList();
            int totalQuestion = question_map.Count();
            int totalPages = (int)Math.Ceiling(totalQuestion / (float)size);
            var output = new QuestionOutputViewModel
            {
                Paging = new Paging(totalQuestion, pagenumber, size, totalPages),
                Questions = question
            };
            return Ok(output);
        }

        // PUT: api/EvaluationQuestions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvaluationQuestions([FromRoute] int id, [FromBody] EvaluationQuestions evaluationQuestions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != evaluationQuestions.EvaluationId)
            {
                return BadRequest();
            }

            _context.Entry(evaluationQuestions).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EvaluationQuestionsExists(id))
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

        // POST: api/EvaluationQuestions
        [HttpPost]
        public async Task<IActionResult> PostEvaluationQuestions([FromBody] EvaluationQuestions evaluation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            evaluation.Date = DateTime.Now;
            _context.EvaluationQuestions.Add(evaluation);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        // DELETE: api/EvaluationQuestions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvaluationQuestions([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var evaluationQuestions = await _context.EvaluationQuestions.FindAsync(id);
            if (evaluationQuestions == null)
            {
                return NotFound();
            }

            _context.EvaluationQuestions.Remove(evaluationQuestions);
            await _context.SaveChangesAsync();

            return Ok(evaluationQuestions);
        }
        [HttpGet("comments/{id}")]
        public async Task<IActionResult> GetComments(int id)
        {
            var comment = await _context.Comments.Include(p => p.User).Where(p => p.CommentId == id).SingleOrDefaultAsync();
            if (comment == null)
            {
                return NotFound();
            }
            var comment_map = _mapper.Map<CommentsViewModel>(comment);
            return Ok(comment_map);
        }
        [HttpPost("comments")]
        public async Task<IActionResult> PostComment(Comments comments)
        {
            comments.Date = DateTime.Now;
            _context.Comments.Add(comments);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            var comment = await _context.Comments.Include(p => p.User).Where(p => p.CommentId == comments.CommentId).SingleOrDefaultAsync();
            var comment_map = _mapper.Map<CommentsViewModel>(comment);

            return CreatedAtAction("GetComments", new { id = comments.CommentId },comment_map);
        }
        private bool EvaluationQuestionsExists(int id)
        {
            return _context.EvaluationQuestions.Any(e => e.EvaluationId == id);
        }
    }
}