using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public class Orders
    {
        public Orders()
        {
            OrderDetails = new List<OrderDetails>();
        }

        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public int Status { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Street { get; set; }
        public decimal? TotalPrice { get; set; }
        public Guid? UserId { get; set; }
        [JsonIgnore]
        public UserInfo User { get; set; }
        public OrderStatuses OrderStatus { get; set; }

        public IList<OrderDetails> OrderDetails { get; set; }
    }
}
