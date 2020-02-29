using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace ClientIdentityServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();//必须注入
            //IDS4注入服务
            var builder = services.AddIdentityServer()
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(Config.GetUsers())
                .AddInMemoryIdentityResources(Config.GetIdentityResources());
            //开发者默认签名证书
            builder.AddDeveloperSigningCredential();

            services.AddAuthentication()
                .AddQQ("QQ", opt =>
                 {
                     opt.AppId = "101853085";
                     opt.AppKey = "035428228cebdbc5ce973fe4f3e1ee2c";
                 })
                .AddOpenIdConnect("oidc", "Demo IdentityServer", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.SaveTokens = true;

                    options.Authority = "https://demo.identityserver.io/";
                    options.ClientId = "native.code";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            //IDS4注入中间件
            app.UseIdentityServer();

            app.UseRouting();
            app.UseAuthorization();//必须注入
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
                endpoints.MapDefaultControllerRoute();//改成默认的路由配置
            });
        }
    }
}
