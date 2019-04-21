using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public class UserInfo
    {
        public UserInfo()
        {
            Address = new HashSet<Address>();
            Orders = new HashSet<Orders>();
            EvaluationQuestions = new HashSet<EvaluationQuestions>();
            CartDetails = new HashSet<CartDetails>();
            Comments = new HashSet<Comments>();
        }

        public string FullName { get; set; }       
        public DateTime BirthDate { get; set; }
        public bool? Gender { get; set; }
        public Guid UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public ICollection<Address> Address { get; set; }
        [JsonIgnore]
        public ICollection<Orders> Orders { get; set; }
        [JsonIgnore]
        public ICollection<EvaluationQuestions> EvaluationQuestions { get; set; }
        [JsonIgnore]
        public ICollection<CartDetails> CartDetails { get; set; }
        [JsonIgnore]
        public ICollection<Comments> Comments { get; set; }
        [JsonIgnore]
        public ICollection<OrdersImportGoods> OrdersImportGoods { get; set; }
    }
}
