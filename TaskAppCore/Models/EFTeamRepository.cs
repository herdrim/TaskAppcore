using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace TaskAppCore.Models
{
    public class EFTeamRepository : ITeamRepository
    {
        private AppIdentityDbContext context;
        private UserManager<AppUser> _userManager;

        public EFTeamRepository(AppIdentityDbContext ctx, UserManager<AppUser> userManager)
        {
            context = ctx;
            _userManager = userManager;
        }

        public IEnumerable<Team> Teams => context.Teams.Include(x => x.Members).Include(x => x.Tasks);

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        public void CreateTeam(Team team, AppUser firstMember = null)
        {
            context.Teams.Add(team);
            if (firstMember != null && firstMember.TeamId == null)
            {
                firstMember.TeamId = team.TeamId;
                context.Users.Update(firstMember);
            }
            context.SaveChanges();
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
            context.Teams.Update(teamToAdd);
            context.SaveChanges();
        }

        public string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string savedPasswordHash = Convert.ToBase64String(hashBytes);

            return savedPasswordHash;
        }
    }
}
