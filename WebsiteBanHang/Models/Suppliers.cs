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
        public int? WardId { get; set; }
        public string Address { get; set; }
        public Wards Wards { get; set; }
        public ICollection<OrdersImportGoods> OrdersImportGoods { get; set; }
    }
}
