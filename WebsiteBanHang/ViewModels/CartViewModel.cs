using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.ViewModels
{
    public class CartViewModel
    {
        public Guid UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public double Discount { get; set; }
        public string Image { get; set; }
        public int Stock { get; set; }
        public bool Discontinued { get; set; }
    }
    public class CartOrderViewModel
    {
        public Guid UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public bool Discontinued { get; set; }
    }
}
