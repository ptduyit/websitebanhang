using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public class User : IdentityUser<Guid>
    {
        //public string Username { get; set; }
        //public string Password { get; set; }
        public bool? Status { get; set; }
        //public Guid UserId { get; set; }

        public UserInfo UserInfo { get; set; }
    }
}
