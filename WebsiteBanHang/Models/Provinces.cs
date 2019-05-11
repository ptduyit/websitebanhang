using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.Models
{
    public class Provinces
    {
        public Provinces()
        {
            Districts = new HashSet<Districts>();
        }
        public int ProvinceId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        [JsonIgnore]
        public ICollection<Districts> Districts { get; set; }
    }
}
