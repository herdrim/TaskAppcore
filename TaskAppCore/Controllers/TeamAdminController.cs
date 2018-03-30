using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskAppCore.Models;
using TaskAppCore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace TaskAppCore.Controllers
{
    [Authorize(Roles = "Admins")]
    public class TeamAdminController : Controller
    {
        private ITeamRepository _teamRepository;
        private UserManager<AppUser> _userManager;

        public TeamAdminController(ITeamRepository teamRepo, UserManager<AppUser> userManager)
        {
            _teamRepository = teamRepo;
            _userManager = userManager;
        }

        public ViewResult Index() => View(_teamRepository.Teams);

        public ViewResult Create() => View();

        [HttpPost]
        public IActionResult Create(TeamCreateModel model)
        {
            if (ModelState.IsValid)
            {
                Team team = new Team
                {
                    Name = model.Name,
                    Password = _teamRepository.HashPassword(model.Password)
                };

                _teamRepository.CreateTeam(team);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int teamId)
        {
            Team team = _teamRepository.Teams.FirstOrDefault(x => x.TeamId == teamId);
            if(team != null)
            {
                _teamRepository.DeleteTeam(team);
            }
            else
            {
                ModelState.AddModelError("", "Team Not Found");
            }

            return RedirectToAction("Index");
            //return View("Index", _teamRepository.Teams);
        }

        public IActionResult Edit(int teamId)
        {
            var team = _teamRepository.Teams.First(t => t.TeamId == teamId);
            List<AppUser> members = new List<AppUser>();
            List<AppUser> nonMembers = new List<AppUser>();
            if (team != null)
            {
                members = _userManager.Users.Where(u => u.TeamId == teamId).ToList();
                nonMembers = _userManager.Users.Where(u => u.TeamId == null).ToList();
            }

            return View(new TeamEditModel
            {
                Team = team,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TeamModificationModel model)
        {            
            if(ModelState.IsValid)
            {
                Team team = _teamRepository.Teams.First(t => t.TeamId == model.TeamId);
                foreach (var m in model.IdsToAdd ?? new string[] { })
                {
                    var user = _userManager.Users.First(u => u.Id == m);
                    user.TeamId = team.TeamId;
                    IdentityResult result = await _userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                        AddErrorsFromResult(result);
                }

                foreach (var m in model.IdsToDelete ?? new string[] { })
                {
                    var user = _userManager.Users.First(u => u.Id == m);
                    user.TeamId = null;
                    IdentityResult result = await _userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                        AddErrorsFromResult(result);
                }
                if(team.Name != model.TeamName)
                {
                    team.Name = model.TeamName;
                    _teamRepository.SaveChanges();
                    
                }
            }

            if(ModelState.IsValid)
                return RedirectToAction(nameof(Index));
            else
                return Edit(model.TeamId);

        }

        // Metoda dodająca errory do modelu
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}