using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.ViewModels
{
    public class UserInfoViewModel
    {
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public bool? Gender { get; set; }
        public Guid UserId { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class RegistrationViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class Password
    {
        public string PassOld { get; set; }
        public string PassNew { get; set; }
    }
}
