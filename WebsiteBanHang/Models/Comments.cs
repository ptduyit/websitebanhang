using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.Models
{
    public class Comments
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int ParentId { get; set; }
        public Guid UserId { get; set; }
        public UserInfo User { get; set; }
        public EvaluationQuestions EvaluationQuestions { get; set; }
    }
}
