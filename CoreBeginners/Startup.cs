using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CoreBeginners.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace CoreBeginners
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuraion)
        {
            _configuration = configuraion;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(options=>
            options.UseSqlServer(_configuration.GetConnectionString("EmployeeConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>(option => {
                option.Password.RequiredLength = 5;
                option.Password.RequiredUniqueChars = 3;
                option.Password.RequireNonAlphanumeric = false;
            
                }).AddEntityFrameworkStores<AppDbContext>();
            services.AddMvc(option => 
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                option.EnableEndpointRouting = false;
                option.Filters.Add(new AuthorizeFilter(policy));
                })/*.AddRazorRuntimeCompilation()*/;
            services.AddRazorPages()
        .AddRazorRuntimeCompilation();

            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
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
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World");
            //});
        }
    }
}
