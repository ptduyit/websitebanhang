using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public partial class Products
    {
        public Products()
        {
            OrderDetails = new HashSet<OrderDetails>();
            OrderImportGoodsDetails = new HashSet<OrderImportGoodsDetails>();
            Replies = new HashSet<Replies>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int? CategoryId { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? ImportPrice { get; set; }
        public bool? Discontinued { get; set; }
        public double? Discount { get; set; }
        public int? Stock { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public byte? Guarantee { get; set; }
        public DateTime? DateUpdated { get; set; }
        public double? Rate { get; set; }

        public ProductCategories Category { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
        public ICollection<OrderImportGoodsDetails> OrderImportGoodsDetails { get; set; }
        public ICollection<Replies> Replies { get; set; }
    }
}
