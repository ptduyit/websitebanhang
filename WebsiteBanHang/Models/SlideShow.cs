using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public class SlideShow
    {
        public int SlideId { get; set; }
        public string Image { get; set; }
        public bool? Status { get; set; }
        public string Link { get; set; }
    }
}
