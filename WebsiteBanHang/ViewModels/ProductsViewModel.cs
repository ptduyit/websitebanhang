using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.ViewModels
{
    public class ProductsViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public double Discount { get; set; }
        public string Image { get; set; }
        public double? Rate { get; set; }
        public ICollection<ProductImages> ProductImage { get; set; }
    }

    public class ProductInformationViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public double Discount { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Guarantee { get; set; }
        public double? Rate { get; set; }
        public string Summary { get; set; }
        public ICollection<ProductImages> ProductImage { get; set; }
    }

    public class ProductShowcaseViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public double Discount { get; set; }
        public string Image { get; set; }
        public float Rate { get; set; }
        public int TotalRate { get; set; }
    }
    public class ProductPriceImportViewModel
    {
        public Products Products { get; set; }
        public List<PriceImport> PriceImport { get; set; }
    }
    public class PriceImport
    {
        public decimal UnitPrice { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
