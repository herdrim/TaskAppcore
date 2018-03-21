using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TaskAppCore.Models
{
    public class EFTaskRepository : ITaskRepository
    {
        private TaskCoreDbContext context;

        public EFTaskRepository(TaskCoreDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Task> Tasks => context.Tasks.Include(x => x.Team).Include(x => x.User);
    }
}
