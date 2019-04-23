﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.ViewModels
{
    public class ProductCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Url { get; set; }
        public int? ParentId { get; set; }
        public List<ProductCategoryViewModel> CategoryChildrens { get; set; }
    }
    public class Breadcrumbs
    {
        public string CategoryName { get; set; }
        public string Url { get; set; }
    }
    public class CategoryOutputViewModel
    {
        public PagingHeader Paging { get; set; }
        public List<ProductCategoryViewModel> Categories { get; set; }
        public List<ProductShowcaseViewModel> Products { get; set; }
        public List<Breadcrumbs> Breadcrumbs { get; set; }
    }
}
