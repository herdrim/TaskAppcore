using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Security.Claims;

namespace TaskAppCore.Models
{
    public class EFTeamRepository : ITeamRepository
    {
        private AppIdentityDbContext _context;
        private UserManager<AppUser> _userManager;

        public EFTeamRepository(AppIdentityDbContext ctx, UserManager<AppUser> userManager)
        {
            _context = ctx;
            _userManager = userManager;
        }

        public IEnumerable<Team> Teams => _context.Teams.Include(x => x.Members).Include(x => x.Tasks);

        public async Task<Team> GetCurrentTeam(ClaimsPrincipal user)
        {
            AppUser usr = await _userManager.GetUserAsync(user);
            Team team = _context.Teams.Include(x => x.Tasks).FirstOrDefault(t => t.TeamId == usr.TeamId);
            return team;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void CreateTeam(Team team, AppUser firstMember = null)
        {
            _context.Teams.Add(team);
            if (firstMember != null && firstMember.TeamId == null)
            {
                firstMember.TeamId = team.TeamId;
                _context.Users.Update(firstMember);
            }
            _context.SaveChanges();
        }

        public void DeleteTeam(Team team)
        {
            foreach (var task in _context.Tasks.Include(t => t.Team).Where(t => t.Team == team))
                _context.Remove(task);
            _context.Teams.Remove(team);
            _context.SaveChanges();
        }

        public void AddMembers(List<AppUser> users, Team team)
        {
            List<AppUser> members;
            var temp = this.Teams.FirstOrDefault(x => x.TeamId == team.TeamId).Members;
            if (temp == null)
                members = new List<AppUser>();
            else
                members = temp.ToList();

            members.AddRange(users);
            var teamToAdd = Teams.FirstOrDefault(x => x.TeamId == team.TeamId);

            foreach (var member in members)
            {
                teamToAdd.Members.Add(member);
            }
            _context.Teams.Update(teamToAdd);
            _context.SaveChanges();
        }


        public void CancelUserTasks(Team team, AppUser user)
        {
            var tasks = team.Tasks.Where(t => t.UserId == user.Id);
            foreach(var t in tasks)
            {
                t.UserId = null;
            }
            _context.Tasks.UpdateRange(tasks);
            _context.SaveChanges();
        }

        public string HashPassword(string password)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
