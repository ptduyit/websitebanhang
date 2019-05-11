using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.ViewModels
{
    public class ShowAddressListViewModel
    {
        public int AddressId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public bool IsDefault { get; set; }
        public Location Location { get; set; }
    }
    public class Location
    {
        public string Ward { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
    }
}
