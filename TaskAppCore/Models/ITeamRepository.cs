using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TaskAppCore.Models
{
    public interface ITeamRepository
    {
        IEnumerable<Team> Teams { get; }

        Task<Team> GetCurrentTeam(ClaimsPrincipal user);

        void SaveChanges();

        string HashPassword(string password);

        void AddMembers(List<AppUser> users, Team team);

        void CreateTeam(Team team, AppUser firstMember = null);

        void DeleteTeam(Team team);

        void CancelUserTasks(Team team, AppUser user);
    }
}
