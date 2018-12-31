using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public class Events
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
    }
}
