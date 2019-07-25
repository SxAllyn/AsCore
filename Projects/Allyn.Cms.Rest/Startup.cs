using Allyn.Cms.Rest.Handlers;
using Allyn.Cms.Rest.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Allyn.Cms.Rest
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {

        /// <summary>
        /// 
        /// </summary>
        private readonly IConfiguration _config;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            #region Redis

            services.AddDistributedRedisCache(o =>
            {
                o.InstanceName = _config?.GetValue<string>("Redis:InstanceName"); 
                o.Configuration = _config?.GetValue<string>("Redis:Configuration");

            });

            #endregion

            #region Cors

            services.AddCors(o =>
            {
                o.AddPolicy("AsMgrCors", p =>
                {
                    p.WithOrigins("http://mgr.allyn.com.cn","https://mgr.allyn.com.cn")
                     .WithMethods("get", "post", "put", "delete")
                     .WithHeaders("authorization", "content-type");
                });
            });

            #endregion

            #region Jwt Authentication

            services.AddAuthorization(c =>{
                c.AddPolicy("AsMgrPolicy", p => p.Requirements.Add(new PolicyRequirement()));
            }).AddAuthentication(c => {
                c.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                c.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                c.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(c =>{
                c.TokenValidationParameters = new TokenValidationParameters {
                    ValidIssuer = _config.GetValue<string>("Jwt:Issuer"),//颁发者
                    ValidAudience = _config.GetValue<string>("Jwt:Audience"), //颁发给谁
                    ClockSkew = TimeSpan.FromMinutes(_config.GetValue<double>("Jwt:ClockSkew")),//允许服务器时间偏移
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Jwt:SecurityKey"))),  //签名密匙
                    ValidateLifetime = true,  //是否验证令牌是否有效
                    //RequireSignedTokens = true,
                    //SaveSigninToken = false,
                    //ValidateActor = false,
                    //ValidateAudience = true,
                    //ValidateIssuer = true,
                    //ValidateIssuerSigningKey = false,
                    //RequireExpirationTime = true,
                };
            });

            services.AddSingleton<IAuthorizationHandler, PolicyHandler>();

            #endregion

            #region Swagger

            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new Info
                {
                    Version = "v1.1.0",
                    Title = "AsMgr Api",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Allyn",
                        Email = "12161792@qq.com",
                        Url = "https://www.allyn.cn"
                    }
                });

                //添加xml注释
                o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Allyn.Cms.Rest.xml"), false);
                o.DocumentFilter<ApiDocTag>();

                //添加header验证信息
                var security = new Dictionary<string, IEnumerable<string>> { { "Bearer", default }, };
                o.AddSecurityRequirement(security);//添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。
                o.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization (data will be transmitted in the request header) parameter structure: Authorization: 'Bearer {token}'",
                    Name = "Authorization",//jwt默认的参数名称
                    In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey"
                });
            });

            #endregion

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseHsts();
            }

            app.UseCors("AsMgrCors");

            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(o =>
            {
                o.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelper v1");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller}/{action}/{id?}"
                  );

                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}");
            });
        }
    }
}
