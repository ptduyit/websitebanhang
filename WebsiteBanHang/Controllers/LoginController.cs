using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebsiteBanHang.Helpers;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;

namespace WebsiteBanHang.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly SaleDBContext _context;

        public LoginController(UserManager<User> userManager, IOptions<JwtIssuerOptions> jwtOptions, SaleDBContext context)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
            _context = context;
        }

        // POST api/Login
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CredentialsViewModel credentials)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 400,
                    Message = "Sai dữ liệu đầu vào"
                });
            }
            var user = await _userManager.FindByNameAsync(credentials.UserName);
            if (user == null)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Tài khoản hoặc mật khẩu không tồn tại"
                });
            }
            bool result = await _userManager.CheckPasswordAsync(user, credentials.Password);
            if (!result)
            {
                return Ok(new Response
                {
                    IsError = true,
                    Status = 404,
                    Message = "Tài khoản hoặc mật khẩu không tồn tại"
                });
            }
            var userInfo = await _context.UserInfo.FindAsync(user.Id);
            var jwt = await Tokens.GenerateJwt(user, userInfo.FullName, _jwtOptions,_userManager);
            return Ok(new Response
            {
                Status = 200,
                Module = jwt
            });
        }

    }
}