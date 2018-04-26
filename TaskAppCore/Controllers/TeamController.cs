using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskAppCore.Models;
using TaskAppCore.Models.ViewModels;

namespace TaskAppCore.Controllers
{
    [Authorize]
    public class TeamController : Controller
    {
        //IUserRepository _userRepository;
        ITeamRepository _teamRepository;
        UserManager<AppUser> _userManager;

        public TeamController(IUserRepository userRepository, UserManager<AppUser> userManager, ITeamRepository teamRepository)
        {
            //_userRepository = userRepository;
            _userManager = userManager;
            _teamRepository = teamRepository;
        }


        public async Task<IActionResult> Index()
        {
            return View(new TeamJoinViewModel
            {
                Teams = _teamRepository.Teams.Where(t => t.TeamId != _teamRepository.GetCurrentTeam(User).Result.TeamId).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(TeamJoinViewModel model)
        {
            if (ModelState.IsValid)
            {
                string password = _teamRepository.HashPassword(model.Password);
                if (_teamRepository.Teams.FirstOrDefault(t => t.TeamId == model.TeamId && t.Password == password) != null)
                {
                    AppUser user = await _userManager.GetUserAsync(User);
                    Team team = _teamRepository.GetCurrentTeam(User).Result;
                    user.TeamId = model.TeamId;
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        _teamRepository.CancelUserTasks(team, user);
                        if (team.Members == null)
                            _teamRepository.DeleteTeam(team);

                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError(nameof(TeamJoinViewModel.Password), "Invalid password");
            }
            model.Teams = _teamRepository.Teams.ToList();
            return View(model);
        }

        public async Task<IActionResult> MyTeam()
        {
            AppUser user = await _userManager.GetUserAsync(User);

            if (user.TeamId != null)
                return View(user.Team);
            else
                return RedirectToAction("Index");
        }
    }
}
