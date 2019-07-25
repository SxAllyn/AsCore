using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Allyn.Cms.Rest.Handlers
{
    /// <summary>
    /// Custom Policy Authorization
    /// </summary>
    public class PolicyHandler : AuthorizationHandler<PolicyRequirement>
    {
        private readonly IConfiguration _config;
        private readonly IDistributedCache _cache;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="config">System configuration</param>
        /// <param name="cache">Redis distributed cache</param>
        public PolicyHandler(IConfiguration config, IDistributedCache cache)
        {
            _cache = cache;
            _config = config;
        }

        /// <summary>
        /// Makes a decision if authorization is allowed based on a specific requirement.
        /// </summary>
        /// <param name="context">The authorization context.</param>
        /// <param name="requirement">The requirement to evaluate.</param>
        /// <returns>Not return async</returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PolicyRequirement requirement)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                HttpContext httpContext = (context.Resource as AuthorizationFilterContext).HttpContext;


                Guid userKey = new Guid(context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
                string cacheToken = await _cache?.GetStringAsync($"{KeyPrefix.CacheJwtTokenKey}{userKey.ToString()}");

                //判断分布式缓是否有当前登录用户的令牌,有:已登录,否:已吊销令牌.
                if (cacheToken != null && cacheToken.Length > 0)
                {
                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(cacheToken);
                    long ticks = long.Parse($"{jwtToken.Claims.FirstOrDefault(o => o.Type == JwtClaimTypes.Expiration).Value}0000000");
                    TimeSpan timeSpan = new TimeSpan(ticks);
                    DateTime startTime = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);
                    DateTime expires = startTime.Add(timeSpan);

                    //Todo 业务授权逻辑 .....
                    context.Succeed(requirement);







                    //如果在服务器允许偏移时间内过期,则刷新令牌.
                    if (expires < DateTime.UtcNow)
                    {
                        string id = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                        string role = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                        await RefreshAndChechTokenAsync(id, httpContext.User.Identity.Name, role, httpContext);
                    }
                    else
                    {
                        StringValues headerAuthorization;
                        if (httpContext.Request.Headers.TryGetValue("authorization", out headerAuthorization))
                        {
                            string headerToken = headerAuthorization.SingleOrDefault()?.Split(" ").Last();

                            //如果缓存的令牌和用户发送的令牌不一致,返回缓存中已刷新的令牌给用户
                            if (cacheToken != headerToken)
                            {
                                httpContext.Response.Headers.Add("refreshToken", cacheToken);
                            }
                        }
                        else
                        {
                            context.Fail();
                        }
                    }
                }
                else
                {
                    context.Fail();
                }
            }
            else
            {
                context.Fail();
            }
        }

        /// <summary>
        /// Refresh the jwt token an caching.
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="name">User name</param>
        /// <param name="role">User role</param>
        /// <param name="httpContext">Current http context</param>
        /// <returns>new token</returns>
        private async Task RefreshAndChechTokenAsync(string id, string name, string role, HttpContext httpContext)
        {
            try
            {
                SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Jwt:SecurityKey")));

                var identity = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier,id),
                    new Claim(ClaimTypes.Name,name),
                    new Claim(ClaimTypes.Role,role)
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
                await _cache.SetStringAsync($"{KeyPrefix.CacheJwtTokenKey}{id}", token, new DistributedCacheEntryOptions { AbsoluteExpiration = expires.AddMinutes(clockSkew) })
                    .ContinueWith(t => httpContext.Response.Headers.Add("refreshToken", token))
               ;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
