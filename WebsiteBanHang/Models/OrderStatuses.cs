using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.Models
{
    public class OrderStatuses
    {
        public OrderStatuses()
        {
            Orders = new HashSet<Orders>();
        }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        [JsonIgnore]
        public ICollection<Orders> Orders { get; set; }
    }
}
