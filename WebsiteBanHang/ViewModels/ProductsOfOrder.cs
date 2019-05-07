using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.ViewModels
{
    public class ProductsOfOrder
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalUnitPrice { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
