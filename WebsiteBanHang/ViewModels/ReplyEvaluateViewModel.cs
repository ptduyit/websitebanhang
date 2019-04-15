using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.ViewModels
{
    public class ReplyEvaluateViewModel
    {
        public int ReplyId { get; set; }
        public DateTime ReplyDate { get; set; }
        public string ReplyContent { get; set; }
        public int? ReplyByReply { get; set; }
        public int ProductId { get; set; }
        public int? Likes { get; set; }
        public bool IsRate { get; set; }
        public int? Rate { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public ICollection<Replies> InverseReplyByReplyNavigation { get; set; }
    }
    public class PagingHeader
    {
        public PagingHeader(
           int totalItems, int pageNumber, int pageSize, int totalPages)
        {
            this.TotalItems = totalItems;
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.TotalPages = totalPages;
        }

        public int TotalItems { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages { get; }

        public string ToJson() => JsonConvert.SerializeObject(this,
                                    new JsonSerializerSettings
                                    {
                                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                                    });

    }
    
    public class EvaluationOutPutViewModel
    {
        public PagingHeader Paging { get; set; }
        public int[] Star { get; set; }
        public List<ReplyEvaluateViewModel> Items { get; set; }
    }
}
