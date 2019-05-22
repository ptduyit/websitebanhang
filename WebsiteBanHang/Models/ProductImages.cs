using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.Models
{
    public class ProductImages
    {
        public int ImageId { get; set; }
        public string Url { get; set; }
        public int? ProductId { get; set; }
        public bool IsThumbnail { get; set; }
        public DateTime CreateAt { get; set; }
        [JsonIgnore]
        public Products Product { get; set; }
    }
}
