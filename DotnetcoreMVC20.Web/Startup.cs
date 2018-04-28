using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using DotnetcoreMVC20.Web.Data;
using DotnetcoreMVC20.Web.Models;
using DotnetcoreMVC20.Web.Services;
using DotnetCore.DAL;
using DotNetCore.Service;
using DotnetcoreMVC20.Web.Models.IoC;
using System.Reflection;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace DotnetcoreMVC20.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //支持IIS
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });

            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = "localhost";//redis连接字符串

            });

            services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            //链接到sql server数据库
            services.AddDbContextPool<SchoolContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ConsumerConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //CustomerService

            services.AddUnitOfWork<SchoolContext>();

            services.AddMvc();
            return InitIoC(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //初始化数据库
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                //自动迁移
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                var contextSchoolContext = serviceScope.ServiceProvider.GetService<SchoolContext>();

                //if (context.Database.GetPendingMigrations().Any())
                if (context.Database.EnsureCreated())
                {
                    Console.WriteLine("Migrating...");
                    //执行迁移
                    context.Database.EnsureCreated();
                    context.Database.Migrate();
                    Console.WriteLine("Migrated End");
                }
                //if (contextSchoolContext.Database.GetPendingMigrations().Any())
                if (context.Database.EnsureCreated())
                {
                    Console.WriteLine("Migrating...");
                    //执行迁移
                    contextSchoolContext.Database.Migrate();
                    Console.WriteLine("Migrated End");
                }

            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// IoC初始化
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private IServiceProvider InitIoC(IServiceCollection services)
        {
            IoCContainer.Register(new Dictionary<string, string>() { { "DotNetCore.Service", "Service" } });
            IoCContainer.Register<IDemoService, DemoService>();
            IoCContainer.Register<IEmailSender, AuthMessageSender>();
            IoCContainer.Register<ISmsSender, AuthMessageSender>();
            return IoCContainer.Build(services);
        }
    }
}