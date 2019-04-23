using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public class Products
    {
        public Products()
        {
            OrderDetails = new HashSet<OrderDetails>();
            OrderImportGoodsDetails = new HashSet<OrderImportGoodsDetails>();
            EvaluationQuestions = new HashSet<EvaluationQuestions>();
            CartDetails = new HashSet<CartDetails>();
            ProductImages = new HashSet<ProductImages>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ImportPrice { get; set; }
        public bool Discontinued { get; set; }
        public double Discount { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Guarantee { get; set; }
        public DateTime DateUpdated { get; set; }
        public double? Rate { get; set; }
        public string Summary { get; set; }
        public bool DisplayIndex { get; set; }
        
        public ProductCategories Category { get; set; }
        [JsonIgnore]
        public ICollection<OrderDetails> OrderDetails { get; set; }
        [JsonIgnore]
        public ICollection<OrderImportGoodsDetails> OrderImportGoodsDetails { get; set; }
        [JsonIgnore]
        public ICollection<EvaluationQuestions> EvaluationQuestions { get; set; }
        [JsonIgnore]
        public ICollection<CartDetails> CartDetails { get; set; }
        public ICollection<ProductImages> ProductImages { get; set; }
    }
}
