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
    [Route("api/[controller]/[action]")]
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

        // GET: api/EvaluationQuestions
        [HttpGet]
        public IEnumerable<EvaluationQuestions> GetEvaluationQuestions()
        {
            return _context.EvaluationQuestions;
        }

        // GET: api/EvaluationQuestions/5
        [HttpGet("{productid}/{pagenumber}")]
        public async Task<IActionResult> GetEvaluation([FromRoute] int productid, [FromRoute] int pagenumber)
        {
            int size = 1;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var evaluationQuestions = await _context.EvaluationQuestions.Include(e => e.User).Include(e => e.Comments).ThenInclude(c => c.User).Where(e => e.ProductId == productid && e.Rate != null).ToListAsync();
            

            if (evaluationQuestions == null)
            {
                return NotFound();
            }
            var eval_map = _mapper.Map<List<EvaluationQuestionsViewModel>>(evaluationQuestions);
            var eval = eval_map.Skip(size * (pagenumber - 1)).Take(size).ToList();
            int totalEval = eval_map.Count();
            int totalPages = (int)Math.Ceiling(totalEval / (float)size);
            var output = new EvaluationOutputViewModel
            {
                Paging = new Paging(totalEval, pagenumber, size, totalPages),
                Star = ListStar(eval_map),
                Evaluations = eval
            };
            return Ok(output);
        }

        public int[] ListStar(List<EvaluationQuestionsViewModel> evaluations)
        {
            int[] rs = new int[5];
            for (int i = 0; i < 5; i++)
            {
                rs[i] = GetStar(evaluations, i + 1);
            }
            return rs;
        }

        public int GetStar(List<EvaluationQuestionsViewModel> evaluations, int star)
        {
            return evaluations.Where(e => e.Rate == star).Count();
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
        public async Task<IActionResult> PostEvaluationQuestions([FromBody] EvaluationQuestions evaluationQuestions)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.EvaluationQuestions.Add(evaluationQuestions);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvaluationQuestions", new { id = evaluationQuestions.EvaluationId }, evaluationQuestions);
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

        private bool EvaluationQuestionsExists(int id)
        {
            return _context.EvaluationQuestions.Any(e => e.EvaluationId == id);
        }
    }
}