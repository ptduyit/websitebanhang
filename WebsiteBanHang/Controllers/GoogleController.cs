using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebsiteBanHang.Auth;
using WebsiteBanHang.Helpers;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;
using static WebsiteBanHang.Models.ExternalApiResponses;

namespace WebsiteBanHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleController : ControllerBase
    {
        private readonly SaleDBContext _appDbContext;
        private readonly UserManager<User> _userManager;
        private readonly GoogleAuthSettings _ggAuthSettings;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private static readonly HttpClient Client = new HttpClient();
        public GoogleController( IOptions<GoogleAuthSettings> ggAuthSettings, UserManager<User> userManager, SaleDBContext appDbContext, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _ggAuthSettings = ggAuthSettings.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AccessTokenViewModel model)
        {
            var userInfoResponse = await Client.GetStringAsync($"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={model.AccessToken}");
            var userInfo = JsonConvert.DeserializeObject<GoogleUserData>(userInfoResponse);
            if(!string.Equals(userInfo.ClientId, _ggAuthSettings.ClientId))
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid google token.", ModelState));
            }
            var user = await _userManager.FindByEmailAsync(userInfo.Email);

            if (user == null)
            {
                var appUser = new User
                {
                    Email = userInfo.Email,
                    UserName = userInfo.Email,
                };

                var result = await _userManager.CreateAsync(appUser, Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8));

                if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

                await _appDbContext.UserInfo.AddAsync(new UserInfo { UserId = appUser.Id, Email = userInfo.Email, FullName = userInfo.Name });
                await _appDbContext.SaveChangesAsync();
                await _userManager.AddToRoleAsync(appUser, "Member");
            }

            // generate the jwt for the local user...
            var localUser = await _userManager.FindByNameAsync(userInfo.Email);

            if (localUser == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Failed to create local user account.", ModelState));
            }

            var jwt = await Tokens.GenerateJwt(_jwtFactory.GenerateClaimsIdentity(localUser.UserName, localUser.Id.ToString()),
              _jwtFactory, localUser.UserName, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });

            return new OkObjectResult(jwt);
        }
    }
}