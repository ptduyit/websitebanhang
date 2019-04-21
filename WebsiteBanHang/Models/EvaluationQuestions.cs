using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public class EvaluationQuestions
    {
        public EvaluationQuestions()
        {
            Comments = new HashSet<Comments>();
        }

        public int EvaluationId { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public int ProductId { get; set; }
        public int? Rate { get; set; }
        public Guid UserId { get; set; }
        [JsonIgnore]
        public Products Product { get; set; }     
        public UserInfo User { get; set; }
        public ICollection<Comments> Comments { get; set; }
    }
}
