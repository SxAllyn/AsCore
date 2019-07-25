using Allyn.Cms.Rest.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Allyn.Cms.Rest.Controllers
{
    /// <summary>
    /// All the user authentication entrance
    /// </summary>
    [Route("[controller]")]
    [Authorize("AsMgrPolicy")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IDistributedCache _cache;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">System configuration</param>
        /// <param name="cache">Redis distributed chache</param>
        public AuthController(IConfiguration config, IDistributedCache cache)
        {
            _config = config;
            _cache = cache;
        }

        /// <summary>
        /// User sign in and get the token
        /// </summary>
        /// <param name="loginer">Logoner information</param>
        /// <returns>Tooken inforation json</returns>
        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<dynamic> SignInAsync(Logoner loginer)
        {
            if (ModelState.IsValid)
            {
                //ToDo 验证用户是否存在
                dynamic user = new 
                {
                    Name = "Allyn",
                    Disabled = false,
                    Id = Guid.NewGuid(),
                    PKey = Guid.Empty,
                    PName = string.Empty,
                    Remarks = "Administrator"
                };

                dynamic role = new { Name = "Administrator" };

                if (user == null) { return StatusCode(StatusCodes.Status428PreconditionRequired, "No account or password error!"); }

                try
                {
                    SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Jwt:SecurityKey")));

                    var identity = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.Name),
                    new Claim(ClaimTypes.Role,role.Name)
                }, JwtBearerDefaults.AuthenticationScheme);

                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                    string issuer = _config.GetValue<string>("Jwt:Issuer");
                    string audience = _config.GetValue<string>("Jwt:Audience");
                    double expiresRelativeMinutes = _config.GetValue<double>("Jwt:Expiration");
                    double clockSkew = _config.GetValue<double>("Jwt:ClockSkew");
                    DateTime expires = DateTime.UtcNow.AddMinutes(expiresRelativeMinutes);

                    JwtSecurityToken jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                        issuer,
                        audience,
                        identity,
                        DateTime.UtcNow,
                        expires,
                        DateTime.UtcNow,
                        new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                    );

                    string token = tokenHandler.WriteToken(jwtSecurityToken);

                    await _cache.SetStringAsync($"{KeyPrefix.CacheJwtTokenKey}{user.Id}", token, new DistributedCacheEntryOptions { AbsoluteExpiration = expires.AddMinutes(clockSkew) });

                    return StatusCode(StatusCodes.Status200OK, token);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest,ModelState);
            }
        }

        /// <summary>
        /// Revoke the token
        /// </summary>
        /// <returns>Not return</returns>
        [HttpPut("SignOut")]
        public async Task SignOutAsync()
        {
            await _cache.RemoveAsync($"{KeyPrefix.CacheJwtTokenKey}{User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value}");
        }
    }
}
