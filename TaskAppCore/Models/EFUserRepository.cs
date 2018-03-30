using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAppCore.Models
{
    public class EFUserRepository : IUserRepository
    {
        private AppIdentityDbContext _context;

        public EFUserRepository(AppIdentityDbContext ctx)
        {
            _context = ctx;
        }

        public IEnumerable<AppUser> Users => _context.Users.Include(x => x.Team).Include(x => x.Tasks);
    }
}
