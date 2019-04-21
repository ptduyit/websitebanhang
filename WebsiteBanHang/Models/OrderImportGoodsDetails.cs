using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public class OrderImportGoodsDetails
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int ProductId { get; set; }

        public OrdersImportGoods Order { get; set; }
        public Products Product { get; set; }
    }
}
