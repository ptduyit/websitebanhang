using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.ViewModels
{
    public class OrdersViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public decimal? TotalPrice { get; set; }
        public Guid? UserId { get; set; }
        public string NameUser { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public string Street { get; set; }
        public string Wards { get; set; }
        public string Provinces { get; set; }
        public string Districts { get; set; }
        public List<OrderDetailsViewModel> OrderDetails { get; set; }
    }
    public class OrderDetailsViewModel
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImages { get; set; }
        public bool Discontinued { get; set; }
    }
    public class OrderOutputViewModel
    {
        public Paging Paging { get; set; }
        public int[] CountOrder { get; set; }
        public List<OrdersViewModel> Orders { get; set; }
    }
    public class HistoryBuy
    {
        public int Percent { get; set; }
        public int TotalOrder { get; set; }
        public int TotalStar { get; set; }
        public float Rate { get; set; }
    }
}
