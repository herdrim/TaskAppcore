using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskAppCore.Models;
using Microsoft.AspNetCore.Identity;
using TaskAppCore.Models.ViewModels;
using Newtonsoft.Json;

namespace TaskAppCore.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        IUserRepository _userRepository;
        ITeamRepository _teamRepository;
        ITaskRepository _taskRepository;
        UserManager<AppUser> _userManager;

        public HomeController(IUserRepository userRepository, UserManager<AppUser> userManager, ITeamRepository teamRepository, ITaskRepository taskRepository)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _teamRepository = teamRepository;
            _taskRepository = taskRepository;
        }
        
        public IActionResult Index()
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            Team team = _userRepository.Users.Where(x => x.Id == userId).Select(x => x.Team).FirstOrDefault();
            if (team.Tasks != null)
                return View(team.Tasks.ToList());

            return View();
        }

        [HttpPost]
        public IActionResult Index(string json)
        {
            TaskViewModel model = JsonConvert.DeserializeObject<TaskViewModel>(json);
            Models.Task task = _taskRepository.Tasks.FirstOrDefault(x => x.TaskId == model.TaskId);
            task.IsToDo = model.IsToDo;
            task.IsInProgress = model.IsInProgress;
            task.IsTested = model.IsTested;
            task.IsFinished = model.IsFinished;

            _taskRepository.Update(task);
            string message = "SUCCESS";
            return Json(new { Message = message });
        }
    }
}