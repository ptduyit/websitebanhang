using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.ViewModels
{
    public class EvaluationQuestionsViewModel
    {
        public int EvaluationId { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public int? Rate { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public List<CommentsViewModel> Comments { get; set; }
    }
    public class CommentsViewModel
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; }
    }
    public class EvaluationOutputViewModel
    {
        public Paging Paging { get; set; }
        public int[] Star { get; set; }
        public List<EvaluationQuestionsViewModel> Evaluations { get; set; }
    }
    public class QuestionOutputViewModel
    {
        public Paging Paging { get; set; }
        public List<EvaluationQuestionsViewModel> Questions { get; set; }
    }
}
