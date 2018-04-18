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

        #region INDEX WITH AJAX SERVING ACTIONS

        public IActionResult Index()
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            int? teamId = _userRepository.Users.FirstOrDefault(x => x.Id == userId).TeamId;
            Team team = _teamRepository.Teams.FirstOrDefault(t => t.TeamId == teamId);
            TaskAssignToUserModel taskViewModel = new TaskAssignToUserModel
            {
                UserId = userId,
                TeamTasks = team.Tasks.ToList(),
                UserTasks = _userRepository.Users.FirstOrDefault(x => x.Id == userId).Tasks.ToList()
            };

            if (team.Tasks != null)
                return View(taskViewModel);

            return View();
        }

        [HttpPost]
        public JsonResult UpdateStatus(string json)
        {
            TaskUpdateJsonModel model = JsonConvert.DeserializeObject<TaskUpdateJsonModel>(json);
            Models.Task task = _taskRepository.Tasks.FirstOrDefault(x => x.TaskId == model.TaskId);
            task.IsToDo = model.IsToDo;
            task.IsInProgress = model.IsInProgress;
            task.IsTested = model.IsTested;
            task.IsFinished = model.IsFinished;

            _taskRepository.Update(task);
            return Json(new { isValid = true });
            //return Json(new { isValid = false, message = "Error while updating data" });
        }

        [HttpPost]
        public JsonResult Assign(string taskId)
        {
            int id;
            if (int.TryParse(taskId, out id))
            {
                Models.Task task = _taskRepository.Tasks.FirstOrDefault(x => x.TaskId == id);
                string userId = _userManager.GetUserId(HttpContext.User);
                if (_userRepository.Users.FirstOrDefault(u => u.Id == userId).Team == task.Team && task.User == null)
                {
                    task.UserId = userId;
                    _taskRepository.Update(task);
                    return Json(new { isValid = true });
                }
            }
            return Json(new { isValid = false, message = "Error while parsing id"});
        }

        [HttpPost]
        public JsonResult CancelTask(string taskId)
        {
            int id;
            if (int.TryParse(taskId, out id))
            {
                Models.Task task = _taskRepository.Tasks.FirstOrDefault(x => x.TaskId == id);
                string userId = _userManager.GetUserId(HttpContext.User);
                if (_userRepository.Users.FirstOrDefault(u => u.Id == userId).Equals(task.User))
                {
                    task.UserId = null;
                    _taskRepository.Update(task);
                    return Json(new { isValid = true });
                }
            }
            return Json(new { isValid = false, message = "Error while parsing id" });
        }
        
        #endregion
    }
}