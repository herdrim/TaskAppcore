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
    public class TaskAdminController : Controller
    {
        ITaskRepository _taskRepository;
        ITeamRepository _teamRepository;
        UserManager<AppUser> _userManager;
        IUserRepository _userRepository;

        public TaskAdminController(ITaskRepository taskRepository, ITeamRepository teamRepository, 
            UserManager<AppUser> userManager, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _teamRepository = teamRepository;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public ViewResult Index() => View(_taskRepository.Tasks.OrderBy(x => x.TeamId));

        public ViewResult Create() => View(new TaskAdminCreateModel { TeamsList = _teamRepository.Teams.ToList() });

        [HttpPost]
        public IActionResult Create(TaskAdminCreateModel model)
        {
            if(ModelState.IsValid)
            {
                Models.Task task = new Models.Task
                {
                    Name = model.Name,
                    Description = model.Description,
                    Deathline = model.Deathline,
                    TeamId = _teamRepository.Teams.FirstOrDefault(x => x.TeamId == model.TeamId).TeamId,
                    IsToDo = true
                };
                _taskRepository.CreateTask(task);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int taskId)
        {
            Models.Task task = _taskRepository.Tasks.FirstOrDefault(x => x.TaskId == taskId);
            if(task != null)
            {
                _taskRepository.DeleteTask(task);
            }
            else
            {
                ModelState.AddModelError("", "Task Not Found");
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int taskId)
        {
            Models.Task task = _taskRepository.Tasks.FirstOrDefault(x => x.TaskId == taskId);

            return View(new TaskAdminEditModel
            {
                Task = task,
                UsersList = _userRepository.Users.Where(x => x.TeamId != null && x.Id != task.UserId).ToList()
            });
        }

        [HttpPost]
        public IActionResult Edit(TaskAdminModificationModel model)
        {
            Models.Task task = _taskRepository.Tasks.FirstOrDefault(x => x.TaskId == model.TaskId);

            if (ModelState.IsValid)
            {
                task.Name = model.Name;
                task.Description = model.Description;
                task.Deathline = model.Deathline;
                task.TeamId = _userRepository.Users.FirstOrDefault(x => x.Id == model.UserId).TeamId;
                task.UserId = _userRepository.Users.FirstOrDefault(x => x.Id == model.UserId).Id;
                
                _taskRepository.SaveChanges();
            }

            if (ModelState.IsValid)
                return RedirectToAction("Index");
            else
                return Edit(model.TaskId);
        }
    }
}
