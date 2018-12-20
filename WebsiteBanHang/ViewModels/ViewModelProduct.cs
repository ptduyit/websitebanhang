using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.ViewModels
{
    public class ViewModelProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal? UnitPrice { get; set; }
        public double? Discount { get; set; }
        public string Image { get; set; }
        public double? Rate { get; set; }
    }
}
