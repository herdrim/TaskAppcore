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
        //IUserRepository _userRepository;
        ITeamRepository _teamRepository;
        ITaskRepository _taskRepository;
        UserManager<AppUser> _userManager;

        public HomeController(IUserRepository userRepository, UserManager<AppUser> userManager, ITeamRepository teamRepository, ITaskRepository taskRepository)
        {
            //_userRepository = userRepository;
            _userManager = userManager;
            _teamRepository = teamRepository;
            _taskRepository = taskRepository;
        }

        #region INDEX WITH AJAX SERVING ACTIONS

        public async Task<IActionResult> Index()
        {
            AppUser user = await _userManager.GetUserAsync(User);
            Team team = await _teamRepository.GetCurrentTeam(User);
            if (team != null)
            {
                TaskAssignToUserModel taskViewModel = new TaskAssignToUserModel();

                taskViewModel.UserId = user.Id;
                taskViewModel.TeamTasks = team.Tasks.ToList();
                if (user.Tasks != null)
                    taskViewModel.UserTasks = user.Tasks.ToList();

                return View(taskViewModel);
            }
            else if (team == null)
                return RedirectToAction("Index", "Team");
            
            return View(new TaskAssignToUserModel());
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
        public async Task<JsonResult> Assign(string taskId)
        {
            int id;
            if (int.TryParse(taskId, out id))
            {
                Models.Task task = _taskRepository.Tasks.FirstOrDefault(x => x.TaskId == id);
                AppUser user = await _userManager.GetUserAsync(User);
                if (user.Team == task.Team && task.User == null)
                {
                    task.UserId = user.Id;
                    _taskRepository.Update(task);
                    return Json(new { isValid = true });
                }
            }
            return Json(new { isValid = false, message = "Error while parsing id"});
        }

        [HttpPost]
        public async Task<JsonResult> CancelTask(string taskId)
        {
            int id;
            if (int.TryParse(taskId, out id))
            {
                Models.Task task = _taskRepository.Tasks.FirstOrDefault(x => x.TaskId == id);
                AppUser user = await _userManager.GetUserAsync(User);
                if (user.Equals(task.User))
                {
                    task.UserId = null;
                    _taskRepository.Update(task);
                    return Json(new { isValid = true });
                }
            }
            return Json(new { isValid = false, message = "Error while parsing id" });
        }

        #endregion

        public IActionResult CreateTask() => View();

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskCreateModel model)
        {
            if(ModelState.IsValid)
            {
                Models.Task task = new Models.Task()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Deathline = model.Deathline,
                    IsToDo = true,
                    Team = await _teamRepository.GetCurrentTeam(User)
                };

                _taskRepository.AddTask(task);

                return RedirectToAction("Index");
            }

            return View(model);           
        }

        public IActionResult TaskList()
        {
            return View(_teamRepository.GetCurrentTeam(User).Result.Tasks);
        }

        [HttpPost]
        public IActionResult DeleteTask(int taskId)
        {
            Models.Task task = _taskRepository.Tasks.FirstOrDefault(t => t.TaskId == taskId);

            if(task != null)
            {
                if(task.TeamId == _teamRepository.GetCurrentTeam(User).Result.TeamId)
                {
                    _taskRepository.DeleteTask(task);
                    return View("TaskList", _teamRepository.GetCurrentTeam(User).Result.Tasks);
                }
            }

            ModelState.AddModelError("", "Task not found");
            return View("TaskList", _teamRepository.GetCurrentTeam(User).Result.Tasks);
        }

        // TO DO
        //public IActionResult Edit(int taskId)
        //{

        //}
    }
}