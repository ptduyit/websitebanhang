using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public partial class Address
    {
        public int AddressId { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string ApartmentNumber { get; set; }
        public Guid? UserId { get; set; }

        public UserInfo User { get; set; }
    }
}
