using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAppCore.Models
{
    public class EFTaskRepository : ITaskRepository
    {
        private TaskCoreDbContext context;

        public EFTaskRepository(TaskCoreDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Task> Tasks => context.Tasks;
    }
}
