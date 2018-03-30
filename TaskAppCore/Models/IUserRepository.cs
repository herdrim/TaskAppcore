using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAppCore.Models
{
    public interface IUserRepository
    {
        IEnumerable<AppUser> Users { get; }
    }
}
