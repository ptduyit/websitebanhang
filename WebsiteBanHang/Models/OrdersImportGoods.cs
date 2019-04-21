using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public class OrdersImportGoods
    {
        public OrdersImportGoods()
        {
            OrderImportGoodsDetails = new HashSet<OrderImportGoodsDetails>();
        }

        public int OrderId { get; set; }
        public int? SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid? UserId { get; set; }
        public decimal? TotalPrice { get; set; }

        public UserInfo User { get; set; }
        public Suppliers Supplier { get; set; }
        public ICollection<OrderImportGoodsDetails> OrderImportGoodsDetails { get; set; }
    }
}
