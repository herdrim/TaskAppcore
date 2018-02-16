using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAppCore.Models
{
    public class EFTeamRepository : ITeamRepository
    {
        private TaskCoreDbContext context;

        public EFTeamRepository(TaskCoreDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Team> Teams => context.Teams;
    }
}
