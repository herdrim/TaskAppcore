using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAppCore.Models
{
    public class TaskCoreDbContext : DbContext
    {
        public TaskCoreDbContext(DbContextOptions<TaskCoreDbContext> options) : base(options)
        { }
        
        public DbSet<Team> Teams { get; set; }
        public DbSet<Task> Tasks { get; set; }
    }
}
