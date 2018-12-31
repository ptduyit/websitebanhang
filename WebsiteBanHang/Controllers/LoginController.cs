using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByNameAsync(credentials.UserName);
            if (user == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password.", ModelState));
            }
            var userInfo = await _context.UserInfo.FindAsync(user.Id);
            var jwt = await Tokens.GenerateJwt(user, userInfo.FullName, _jwtOptions,_userManager);
            return new OkObjectResult(jwt);
        }

    }
}