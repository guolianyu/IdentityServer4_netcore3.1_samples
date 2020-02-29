using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace mvcclient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = "Cookies";
                opt.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies", opt =>
             {
                 opt.Cookie.Name = "louiecookiemvc";

             })
            .AddOpenIdConnect("oidc", opt =>
             {
                 opt.Authority = "http://localhost:5000";
                 opt.RequireHttpsMetadata = false;
                 opt.ClientId = "client";//这个代表应用程序注册到认证服务中心的客户端ID
                 opt.SaveTokens = true;
                 //opt.ResponseType = "code";
                 //opt.ClientSecret = "louieclient";
             });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                //.RequireAuthorization();
            });
        }
    }
}
