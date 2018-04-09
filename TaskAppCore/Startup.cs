using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using TaskAppCore.Models;
using Microsoft.AspNetCore.Identity;
using TaskAppCore.Infrastructure;

namespace TaskAppCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

       
        public void ConfigureServices(IServiceCollection services)
        {
            // Do własnej walidacji hasła (dwie opcje - sprawdzić w infrastructure -> CustomPasswordValidator
            //services.AddTransient<IPasswordValidator<AppUser>, CustomPasswordValidator>();
            //services.AddTransient<IPasswordValidator<AppUser>, CustomPasswordValidator2>();
            // Do własnej walidacji usera (dwie opcje - sprawdzić w infrastructure -> CustomUserValidator
            //services.AddTransient<IUserValidator<AppUser>, CustomUserValidator>();
            //services.AddTransient<IUserValidator<AppUser>, CustomUserValidator2>();

            //services.AddDbContext<TaskCoreDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("TaskCoreDb")));

            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("TaskCoreDb")));

            // Dodanie identity(możliwość zmiany domyślnych reguł) - 
            // AppUser reprezentuje użytkownika(własny model), a IdentityRole role(model domyślny)
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppIdentityDbContext>();


            services.AddTransient<IUserRepository, EFUserRepository>();
            services.AddTransient<ITaskRepository, EFTaskRepository>();
            services.AddTransient<ITeamRepository, EFTeamRepository>();
            services.AddMvc();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            app.UseStaticFiles();
            //app.UseIdentity();
            app.UseAuthentication();
            app.UseMvc(options =>
            options.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"));
        }
    }
}
