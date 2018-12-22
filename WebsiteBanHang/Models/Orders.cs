using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public partial class Orders
    {
        public Orders()
        {
            OrderDetails = new HashSet<OrderDetails>();
        }

        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int? Status { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string ApartmentNumber { get; set; }
        public decimal? TotalPrice { get; set; }
        public Guid? UserId { get; set; }
        [JsonIgnore]
        public UserInfo User { get; set; }

        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
