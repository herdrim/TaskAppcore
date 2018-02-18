using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskAppCore.Models;

namespace TaskAppCore.Controllers
{
    public class HomeController : Controller
    {
        ITeamRepository _teamRepository;
        public int PageSize = 2;

        public HomeController(ITeamRepository repo)
        {
            _teamRepository = repo;
        }

        [Authorize]
        public IActionResult Index() => View(_teamRepository.Teams.Where(t => t.TeamId == 1).FirstOrDefault());
    }
}