using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.Models
{
    public class Districts
    {
        public Districts()
        {
            Wards = new HashSet<Wards>();
        }
        public int DistrictId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int ProvinceId { get; set; }
        [JsonIgnore]
        public ICollection<Wards> Wards { get; set; }

        public Provinces Provinces { get; set; }
    }
}
