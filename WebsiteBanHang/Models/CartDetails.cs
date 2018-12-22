using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.Models
{
    public class CartDetails
    {
        public Guid UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        [JsonIgnore]
        public Products Product { get; set; }
        [JsonIgnore]
        public UserInfo UserInfo { get; set; }
    }
}
