using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public class Suppliers
    {
        public Suppliers()
        {
            OrdersImportGoods = new HashSet<OrdersImportGoods>();
        }

        public int SupplierId { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }

        public ICollection<OrdersImportGoods> OrdersImportGoods { get; set; }
    }
}
