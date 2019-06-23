using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        public Rating Rating { get; set; }
        public List<EvaluationQuestionsViewModel> Evaluations { get; set; }
    }
    public class Rating
    {
        public Rating(float star, int total, int[] starList)
        {
            this.Star = star;
            this.Total = total;
            this.StarList = starList;
        }

        public float Star { get; set; }
        public int Total { get; set; }
        public int[] StarList { get; set; }

        public string ToJson() => JsonConvert.SerializeObject(this,
                                    new JsonSerializerSettings
                                    {
                                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                                    });
    }
    public class QuestionOutputViewModel
    {
        public Paging Paging { get; set; }
        public List<EvaluationQuestionsViewModel> Questions { get; set; }
    }
    public class ProductOrderViewModel
    {
        public DateTime OrderDate { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
    }
    public class ProductNotReview
    {
        public Paging Paging { get; set; }
        public List<ProductOrderViewModel> Products { get; set; }
    }
    public class ProductHistoryEvaluationViewModel
    {
        public DateTime OrderDate { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public int Rate { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int EvaluationId { get; set; }
    }
    public class ProductReviewHistory
    {
        public Paging Paging { get; set; }
        public List<ProductHistoryEvaluationViewModel> Products { get; set; }
    }
}
