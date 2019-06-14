using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebsiteBanHang.ViewModels
{
    public class Response
    {
        public dynamic Module { get; set; }
        public int Status { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
    }
}
