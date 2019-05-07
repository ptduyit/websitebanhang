using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.ViewModels
{
    public class TotalProductSold
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalQuantity { get; set; }
    }
}
