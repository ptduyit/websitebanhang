using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public partial class UserInfo
    {
        public UserInfo()
        {
            AddressNavigation = new HashSet<Address>();
            Orders = new HashSet<Orders>();
            Replies = new HashSet<Replies>();
            CartDetails = new HashSet<CartDetails>();
        }

        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? Gender { get; set; }
        public Guid UserId { get; set; }

        public User User { get; set; }
        public ICollection<Address> AddressNavigation { get; set; }
        public ICollection<Orders> Orders { get; set; }
        public ICollection<Replies> Replies { get; set; }
        [JsonIgnore]
        public ICollection<CartDetails> CartDetails { get; set; }
    }
}
