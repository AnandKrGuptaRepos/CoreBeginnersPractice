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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using CoreBeginners.Security;
using DNTCaptcha.Core;

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
            //This Line for adding service for DbContext class and provide Sql database Connection name for creating Migration. 
            services.AddDbContextPool<AppDbContext>(options=>
            options.UseSqlServer(_configuration.GetConnectionString("EmployeeConnection")));

            //This Line for Adding services for Login/Register and Role Manage to IdentityDBContext  or set Custom Password Rules
            services.AddIdentity<ApplicationUser, IdentityRole>(option => {
                option.Password.RequiredLength = 5;
                option.Password.RequiredUniqueChars = 3;
                option.Password.RequireNonAlphanumeric = false;
                option.SignIn.RequireConfirmedEmail = true;
                option.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

                option.Lockout.MaxFailedAccessAttempts = 3;
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders() //If Add column to Migration and show error "Unable to create an object of type 'AppDBContext'" Then this line will be execute.

            //This Line for Custom Time duration for email Token provider life time
            .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");
            //This line for all token set same life time (Reset password, Email confimation)
            services.Configure<DataProtectionTokenProviderOptions>(o => { o.TokenLifespan = TimeSpan.FromHours(5); });
            services.Configure<CustomEmailConfirmaionTokenProvideOptions>(o=> { o.TokenLifespan = TimeSpan.FromDays(3); });
           
            //This line for Policy based authorization 
            services.AddAuthorization(option=> {
                option.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));
                option.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role","true"));
            });

            // This pipeline is basically used for using Captcha on Login
            services.AddDNTCaptcha(options=>options.UseCookieStorageProvider().ShowThousandsSeparators(false));

            // This Line for Encription and decryption of route value
            services.AddSingleton<DataProtectionPurposeString>();

            // This Line for External Login Providers
             services.AddAuthentication().AddGoogle(o => {
                o.ClientId = "988700148937-u7ararn5cf649v8b2op3rf8rj0bld454.apps.googleusercontent.com";
                o.ClientSecret = "D15grZ7o9wKXawxoZEkDaFck";
            }).AddFacebook(f => {
                f.AppId = "1193480270993429";
                f.AppSecret = "b0ff64b16cbe25654873196b31616ffe";
                //f.AppId = "262659601458765";
                //f.AppSecret = "4cf8caee216b33830a3440a1a51cb418";
            });//.AddTwitter(t=> {
               //    t.ConsumerKey = "988700148937-u7ararn5cf649v8b2op3rf8rj0bld454.apps.googleusercontent.com";
               //    t.ConsumerSecret = "D15grZ7o9wKXawxoZEkDaFck";
               //}).AddMicrosoftAccount(m=> {
               //    m.ClientId = "988700148937-u7ararn5cf649v8b2op3rf8rj0bld454.apps.googleusercontent.com";
               //    m.ClientSecret = "D15grZ7o9wKXawxoZEkDaFck";
               //});

            //This Line for Adding services to Globally Redirect AccessDenied Page if User Not Authorize to use that page.
            services.ConfigureApplicationCookie(c => c.AccessDeniedPath = new PathString("/Administration/AccessDenied"));
            services.AddMvc(option => 
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                option.EnableEndpointRouting = false;
                option.Filters.Add(new AuthorizeFilter(policy));
            })/*.AddRazorRuntimeCompilation()*/;
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
            services.AddSingleton<IList<AuthenticationScheme>,List<AuthenticationScheme>>();
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
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            //});
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
