using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int? WardId { get; set; }
        public string Street { get; set; }
        public bool IsDefault { get; set; }
        public Guid UserId { get; set; }
        [JsonIgnore]
        public UserInfo User { get; set; }
        public Wards Wards { get; set; }
    }
}
