using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TaskAppCore.Models
{
    public class EFTaskRepository : ITaskRepository
    {
        private AppIdentityDbContext _context;

        public EFTaskRepository(AppIdentityDbContext ctx)
        {
            _context = ctx;
        }

        public IEnumerable<Task> Tasks => _context.Tasks.Include(x => x.Team).Include(x => x.User);

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void CreateTask(Task task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        public void DeleteTask(Task task)
        {
            _context.Tasks.Remove(task);
            _context.SaveChanges();
        }

        public void Update(Task task)
        {
            _context.Tasks.Update(task);
            _context.SaveChanges();
        }
    }
}
