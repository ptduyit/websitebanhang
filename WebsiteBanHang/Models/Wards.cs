using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.Models
{
    public class Wards
    {
        public Wards()
        {
            Address = new HashSet<Address>();
            Orders = new HashSet<Orders>();
            Suppliers = new HashSet<Suppliers>();
        }
        public int WardId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int DistrictId { get; set; }

        public Districts Districts { get; set; }
        [JsonIgnore]
        public ICollection<Address> Address { get; set; }
        [JsonIgnore]
        public ICollection<Orders> Orders { get; set; }
        [JsonIgnore]
        public ICollection<Suppliers> Suppliers { get; set; }
    }
}
