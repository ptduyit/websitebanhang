using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public class ProductCategories
    {
        public ProductCategories()
        {
            Products = new HashSet<Products>();
            CategoryChildrens = new HashSet<ProductCategories>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Url { get; set; }
        public int? ParentId { get; set; }
        public ProductCategories CategoryParent { get; set; }
        public ICollection<ProductCategories> CategoryChildrens { get; set; }
        public ICollection<Products> Products { get; set; }
    }
}
