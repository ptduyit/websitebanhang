using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebsiteBanHang.Models;

namespace WebsiteBanHang.Helpers
{
    public class Tokens
    {
        public static async Task<JsonResponse> GenerateJwt(User user, string fullName, JwtIssuerOptions _jwtOptions, UserManager<User> _userManager)
        {
            var claims = new List<Claim>
            {
                 new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                 new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                 new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64)
             };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new JsonResponse
            {
                fullName = fullName,
                id = user.Id,
                token = encodedJwt,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };
            return response;
            //return JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);
    }
    public class JsonResponse
    {
        public string fullName { get; set; }
        public Guid id { get; set; }
        public string token { get; set; }
        public int expires_in { get; set; }
    }
}
