using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAppCore.Models
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TaskCoreDbContext>
    {
        public TaskCoreDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<TaskCoreDbContext>();

            var connectionString = configuration.GetConnectionString("TaskCoreDb");

            builder.UseSqlServer(connectionString);

            return new TaskCoreDbContext(builder.Options);
        }
    }
}
