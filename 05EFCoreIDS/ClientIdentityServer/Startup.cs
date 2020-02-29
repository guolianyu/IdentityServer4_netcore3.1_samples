using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Authentication.QQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
            const string connectionString = "Server=192.168.75.235;database=identityserver;port=3306;uid=root;pwd=mysql123;Character Set=utf8;SslMode = none;";
            services.AddControllersWithViews();//必须注入

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            //IDS4注入服务
            var builder = services.AddIdentityServer(opt =>
            {
                //opt.UserInteraction.LoginUrl = "/account/logout";//设置交互的登录地址，默认account/login
            })
                .AddTestUsers(Config.GetUsers())
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                builder.UseMySql(connectionString,
                    sql => sql.MigrationsAssembly(migrationsAssembly));
                }).AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseMySql(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                });
            //.AddInMemoryApiResources(Config.GetApis())
            //.AddInMemoryClients(Config.GetClients())
            //.AddTestUsers(Config.GetUsers())
            //.AddInMemoryIdentityResources(Config.GetIdentityResources());
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
            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins("https://localhost:5003")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            InitializeDatabase(app);
            app.UseCors("default");
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
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApis())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
