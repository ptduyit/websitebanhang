using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public class OrderDetails
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int ProductId { get; set; }
        [JsonIgnore]
        public Orders Order { get; set; }
        
        public Products Product { get; set; }
    }
}
