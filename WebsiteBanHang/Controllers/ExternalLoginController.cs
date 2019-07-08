using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebsiteBanHang.Helpers;
using WebsiteBanHang.Models;
using WebsiteBanHang.ViewModels;
using static WebsiteBanHang.Models.ExternalApiResponses;

namespace WebsiteBanHang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ExternalLoginController : ControllerBase
    {
        private readonly SaleDBContext _appDbContext;
        private readonly UserManager<User> _userManager;
        private readonly GoogleAuthSettings _ggAuthSettings;
        private readonly FacebookAuthSettings _fbAuthSettings;
        private readonly JwtIssuerOptions _jwtOptions;
        private static readonly HttpClient Client = new HttpClient();

        public ExternalLoginController(IOptions<GoogleAuthSettings> ggAuthSettings, IOptions<FacebookAuthSettings> fbAuthSettingsAccessor
            , UserManager<User> userManager, SaleDBContext appDbContext, IOptions<JwtIssuerOptions> jwtOptions)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _jwtOptions = jwtOptions.Value;
            _ggAuthSettings = ggAuthSettings.Value;
            _fbAuthSettings = fbAuthSettingsAccessor.Value;
        }
        [HttpPost("{platform}")]
        public async Task<IActionResult> Post([FromBody]AccessTokenViewModel model, [FromRoute] string platform)
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
            if (platform == "google")
            {
                var userInfoResponse = await Client.GetStringAsync($"https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={model.AccessToken}");
                var userInfo = JsonConvert.DeserializeObject<UserData>(userInfoResponse);
                if (!string.Equals(userInfo.ClientId, _ggAuthSettings.ClientId))
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 404,
                        Message = "Đăng nhập thất bại"
                    });
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

                    if (!result.Succeeded)
                        return Ok(new Response
                        {
                            IsError = true,
                            Status = 409,
                            Message = "Lỗi khi thêm tài khoản"
                        });

                    await _appDbContext.UserInfo.AddAsync(new UserInfo { UserId = appUser.Id, FullName = userInfo.Name, BirthDate = DateTime.Now });
                    await _appDbContext.SaveChangesAsync();
                    await _userManager.AddToRoleAsync(appUser, "Member");
                }

                // generate the jwt for the local user...
                var localUser = await _userManager.FindByNameAsync(userInfo.Email);

                if (localUser == null)
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 409,
                        Message = "lỗi khi thêm tài khoản"
                    });
                }
                var localUserInfo = await _appDbContext.UserInfo.FindAsync(localUser.Id);

                var jwt = await Tokens.GenerateJwt(localUser, localUserInfo?.FullName ?? "noname", _jwtOptions, _userManager);

                return Ok(new Response
                {
                    Status = 200,
                    Module = jwt
                });
            }
            else if (platform == "facebook")
            {
                // 1.generate an app access token
                var appAccessTokenResponse = await Client.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_fbAuthSettings.AppId}&client_secret={_fbAuthSettings.AppSecret}&grant_type=client_credentials");
                var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessToken>(appAccessTokenResponse);
                // 2. validate the user access token
                var userAccessTokenValidationResponse = await Client.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={model.AccessToken}&access_token={appAccessToken.AccessToken}");
                var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookUserAccessTokenValidation>(userAccessTokenValidationResponse);

                if (!userAccessTokenValidation.Data.IsValid)
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 400,
                        Message = "Invalid facebook token"
                    });
                    
                }

                // 3. we've got a valid token so we can request user data from fb
                var userInfoResponse = await Client.GetStringAsync($"https://graph.facebook.com/v3.2/me?fields=id,email,name,gender,birthday&access_token={model.AccessToken}");
                var userInfo = JsonConvert.DeserializeObject<UserData>(userInfoResponse);
                var user = await _userManager.FindByEmailAsync(userInfo.Email);

                if (user == null)
                {
                    var appUser = new User
                    {
                        Email = userInfo.Email,
                        UserName = userInfo.Email,
                    };

                    var result = await _userManager.CreateAsync(appUser, Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8));

                    if (!result.Succeeded)
                        return Ok(new Response
                        {
                            IsError = true,
                            Status = 409,
                            Message = "lỗi khi thêm tài khoản"
                        });

                    await _appDbContext.UserInfo.AddAsync(new UserInfo { UserId = appUser.Id, FullName = userInfo.Name, BirthDate = DateTime.Now });
                    await _appDbContext.SaveChangesAsync();
                    await _userManager.AddToRoleAsync(appUser, "Member");
                }

                // generate the jwt for the local user...
                var localUser = await _userManager.FindByNameAsync(userInfo.Email);

                if (localUser == null)
                {
                    return Ok(new Response
                    {
                        IsError = true,
                        Status = 409,
                        Message = "lỗi khi thêm tài khoản"
                    });
                    
                }
                var localUserInfo = await _appDbContext.UserInfo.FindAsync(localUser.Id);

                var jwt = await Tokens.GenerateJwt(localUser, localUserInfo?.FullName ?? "noname", _jwtOptions, _userManager);

                return Ok(new Response
                {
                    Status = 200,
                    Module = jwt
                });
            }
            return Ok(new Response
            {
                IsError = true,
                Status = 400,
                Message = "chỉ hỗ trợ google facebook"
            });
        }
    }
}