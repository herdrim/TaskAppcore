using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskAppCore.Models
{
    public static class SeedData
    {
        public static void EnsureData(AppIdentityDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Teams.Any())
            {
                context.Teams.AddRange(                
                    new Team{ Name="team1", Password=HashPassword("team1"), Tasks = null },
                    new Team{ Name="team2", Password= HashPassword("team2"), Tasks = null },
                    new Team{ Name="team3", Password= HashPassword("team3"), Tasks = null }
                );
                context.SaveChanges();
            }

            if (!context.Tasks.Any())
            {
                context.Tasks.AddRange(                
                    new Task{ Name="task1", Description="do sth1", Deathline=new DateTime(2018, 10, 10), StartTime=null, EndTime=null, IsToDo=true, IsInProgress=false, IsTested=false, IsFinished=false, User=null, Team=null },
                    new Task{ Name="task2", Description="do sth2", Deathline=new DateTime(2018, 08, 22), StartTime=null, EndTime=null, IsToDo=true, IsInProgress=false, IsTested=false, IsFinished=false, User=null, Team=null },
                    new Task{ Name="task3", Description="do sth3", Deathline=new DateTime(2018, 02, 10), StartTime=null, EndTime=null, IsToDo=true, IsInProgress=false, IsTested=false, IsFinished=false, User=null, Team=null },
                    new Task{ Name="task4", Description="do sth4", Deathline=new DateTime(2018, 11, 13), StartTime=null, EndTime=null, IsToDo=true, IsInProgress=false, IsTested=false, IsFinished=false, User=null, Team=null }
                );
                context.SaveChanges();
            }
        }

        public static string HashPassword(string password)
        {
            using (System.Security.Cryptography.SHA1Managed sha1 = new System.Security.Cryptography.SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash);
            }
        }


        public static async System.Threading.Tasks.Task CreateAdminAccount(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            string username = configuration["Data:AdminUser:Name"];
            string email = configuration["Data:AdminUser:Email"];
            string password = configuration["Data:AdminUser:Password"];
            string role = configuration["Data:AdminUser:Role"];

            if(await userManager.FindByNameAsync(username) == null)
            {
                if (await roleManager.FindByNameAsync(role) == null)
                    await roleManager.CreateAsync(new IdentityRole(role));

                AppUser user = new AppUser
                {
                    UserName = username,
                    Email = email,
                };

                IdentityResult result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, role);
            }
        }

    }    
}
