using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace Allyn.Cms.Mgr.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration confit)
        {
            _config = confit;
        }

        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SigninAsync(string token)
        {
            try
            {
                SecurityToken securityToken;
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidIssuer = _config.GetValue<string>("Jwt:Issuer"),//颁发者
                    ValidAudience = _config.GetValue<string>("Jwt:Audience"), //颁发给谁
                    ClockSkew = TimeSpan.FromMinutes(_config.GetValue<double>("Jwt:ClockSkew")),//允许服务器时间偏移
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Jwt:SecurityKey"))),  //签名密匙
                    ValidateLifetime = true,  //是否验证令牌是否有效
                }, out securityToken);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                long ticks = long.Parse($"{claimsPrincipal.Claims.FirstOrDefault(o => o.Type == JwtClaimTypes.Expiration).Value}0000000");
                TimeSpan timeSpan = new TimeSpan(ticks);
                DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);
                DateTime expires = startTime.Add(timeSpan);
                HttpContext.Response.Cookies.Append("token", token, new CookieOptions { Expires = expires.AddMinutes(_config.GetValue<double>("Jwt:ClockSkew")) });
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
            }
        }
    }
}