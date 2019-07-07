using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteBanHang.Helpers;
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
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }
            int size = 3;

            var evaluations = await _context.EvaluationQuestions.Include(e => e.User)
                .Include(e => e.Comments).ThenInclude(c => c.User)
                .Where(e => e.ProductId == productid && e.Rate != null)
                .ToListAsync();
            
            if (evaluations == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
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
            return Ok(new Response
            {
                Status = 200,
                Module = output
            });
        }

        [HttpGet("{productid}/{pagenumber}")]
        public async Task<IActionResult> GetEQuestions([FromRoute] int productid, [FromRoute] int pagenumber)
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
            int size = 1;

            var evaluationQuestions = await _context.EvaluationQuestions.Include(e => e.User).Include(e => e.Comments).ThenInclude(c => c.User).Where(e => e.ProductId == productid && e.Rate == null).ToListAsync();

            if (evaluationQuestions == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
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
            return Ok(new Response
            {
                Status = 200,
                Module = output
            });
        }
        [HttpGet("not-review/{userid}")]
        public async Task<IActionResult> GetNotReviewUser(Guid userid, [FromQuery] int page)
        {
            int size = 5;
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }
            var order = await _context.OrderDetails.Include(a => a.Order).Include(a => a.Product).ThenInclude(b =>b.ProductImages)
                .Where(a => a.Order.UserId == userid && a.Order.Status == Globals.DA_GIAO)
                .GroupBy(a => a.ProductId).Select(x => x.First()).ToListAsync();
            var product = _mapper.Map<List<ProductOrderViewModel>>(order);

            var review_history = await _context.EvaluationQuestions.Where(a => a.Rate != null && a.UserId == userid)
                .Select(a => a.ProductId).ToListAsync();
            var product_not_review = product.Where(p => !review_history.Contains(p.ProductId)).ToList();

            int totalItem = product_not_review.Count();
            int totalPages = (int)Math.Ceiling(totalItem / (float)size);
            page = (page < 1) ? 1 : ((page > totalPages) ? totalPages : page);
            var product_page = product_not_review.Skip(size * (page - 1)).Take(size).ToList();
            var output = new ProductNotReview
            {
                Paging = new Paging(totalItem, page, size, totalPages),
                Products = product_page
            };

            return Ok(new Response
            {
                Status = 200,
                Module = output
            });

        }
        [HttpGet("review-history/{userid}")]
        public async Task<IActionResult> GetHistoryReviewUser(Guid userid, [FromQuery] int page)
        {
            int size = 5;
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }
            var review = await _context.EvaluationQuestions.Include(p => p.Product).ThenInclude(i =>i.ProductImages)
                .Where(p => p.Rate != null && p.UserId == userid).ToListAsync();
            var review_map = _mapper.Map<List<ProductHistoryEvaluationViewModel>>(review);

            int totalItem = review_map.Count();
            int totalPages = (int)Math.Ceiling(totalItem / (float)size);
            page = (page < 1) ? 1 : ((page > totalPages) ? totalPages : page);
            var product_page = review_map.Skip(size * (page - 1)).Take(size).ToList();
            var output = new ProductReviewHistory
            {
                Paging = new Paging(totalItem, page, size, totalPages),
                Products = product_page
            };
            return Ok(new Response
            {
                Status = 200,
                Module = output
            });

        }

        // PUT: api/EvaluationQuestions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvaluationQuestions([FromRoute] int id, [FromBody] EvaluationQuestions evaluationQuestions)
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

            if (id != evaluationQuestions.EvaluationId)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
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
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 404,
                        Message = "Không tìm thấy dữ liệu"
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
        [HttpPost("review/add")]
        public async Task<IActionResult> PostPutReview([FromBody] EvaluationQuestions evaluation)
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
            if(evaluation.EvaluationId > 0)
            {
                evaluation.Date = DateTime.Now;
                _context.Entry(evaluation).State = EntityState.Modified; 
            }
            else
            {
                evaluation.Date = DateTime.Now;
                _context.EvaluationQuestions.Add(evaluation);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 409,
                    Message = "Không thể lưu"
                });
            }

            return Ok(new Response
            {
                Status = 204
            });
        }
        // POST: api/EvaluationQuestions
        [HttpPost]
        public async Task<IActionResult> PostEvaluationQuestions([FromBody] EvaluationQuestions evaluation)
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
            evaluation.Date = DateTime.Now;
            _context.EvaluationQuestions.Add(evaluation);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 409,
                    Message = "Không thể lưu"
                });
            }

            return Ok(new Response
            {
                Status = 204
            });
        }

        // DELETE: api/EvaluationQuestions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvaluationQuestions([FromRoute] int id)
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

            var evaluationQuestions = await _context.EvaluationQuestions.FindAsync(id);
            if (evaluationQuestions == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }

            _context.EvaluationQuestions.Remove(evaluationQuestions);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 409,
                    Message = "Không thể lưu"
                });
            }

            return Ok(new Response
            {
                Status = 200,
                Module = evaluationQuestions
            });
        }
        [HttpGet("comments/{id}")]
        public async Task<IActionResult> GetComments(int id)
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
            var comment = await _context.Comments.Include(p => p.User).Where(p => p.CommentId == id).SingleOrDefaultAsync();
            if (comment == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }
            var comment_map = _mapper.Map<CommentsViewModel>(comment);
            return Ok(new Response
            {
                Status = 200,
                Module = comment_map
            });
        }
        [HttpPost("comments")]
        public async Task<IActionResult> PostComment(Comments comments)
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
            comments.Date = DateTime.Now;
            _context.Comments.Add(comments);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 409,
                    Message = "Không thể lưu"
                });
            }
            var comment = await _context.Comments.Include(p => p.User).Where(p => p.CommentId == comments.CommentId).SingleOrDefaultAsync();
            if(comment == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Không tìm thấy dữ liệu"
                });
            }
            var comment_map = _mapper.Map<CommentsViewModel>(comment);
            return Ok(new Response
            {
                Status = 201,
                Module = comment_map
            });

        }
        private bool EvaluationQuestionsExists(int id)
        {
            return _context.EvaluationQuestions.Any(e => e.EvaluationId == id);
        }
    }
}