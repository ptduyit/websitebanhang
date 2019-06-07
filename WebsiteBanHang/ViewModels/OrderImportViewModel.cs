using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.ViewModels
{
    public class OrderImportViewModel
    {
        public int OrderId { get; set; }
        public int? SupplierId { get; set; }
        public string CompanyName { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid? UserId { get; set; }
        public string FullName { get; set; }
        public List<ImportDetailProductViewModel> OrderDetails { get; set; }

    }
    public class OrderImportUpdateViewModel
    {
        public int OrderId { get; set; }
        public int? SupplierId { get; set; }
        public List<OrderImportGoodsDetails>  Product { get; set; }
    }
    
    public class OrderImportFirstViewModel
    {
        public Guid UserId { get; set; }
        public int? SupplierId { get; set; }
        public int ProductId { get; set; }
    }
    public class OrderImportDetailsViewModel
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
    }
    public class ImportDetailProductViewModel
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
