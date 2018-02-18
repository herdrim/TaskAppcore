using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TaskAppCore.Models;
using TaskAppCore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace TaskAppCore.Controllers
{
    [Authorize(Roles = "Admins")]
    public class RoleAdminController : Controller
    {
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<AppUser> _userManager;

        public RoleAdminController(RoleManager<IdentityRole> roleMgr, UserManager<AppUser> usrMgr)
        {
            _roleManager = roleMgr;
            _userManager = usrMgr;
        }

        public IActionResult Index() => View(_roleManager.Roles);

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create([Required]string name)
        {
            if(ModelState.IsValid)
            {
                // stworzenie nowej roli
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    AddErrorsFromResult(result);
            }

            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            // znalezienie roli po Id
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if(role != null)
            {
                // usunięcie roli
                IdentityResult result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    AddErrorsFromResult(result);
            }
            else
            {
                ModelState.AddModelError("", "No role found");
            }

            return View("Index", _roleManager.Roles);
        }
        
        public async Task<IActionResult> Edit(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            List<AppUser> members = new List<AppUser>();
            List<AppUser> nonMembers = new List<AppUser>();

            foreach(AppUser user in _userManager.Users)
            {
                // sprawdzenie czy user ma podaną role - jeżeli tak to zmienna list reprezentuje zmienną members inaczej nonMembers
                var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);
            }

            return View(new RoleEditModel
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleModificationModel model)
        {
            IdentityResult result;
            if(ModelState.IsValid)
            {
                // pętla przechodząca przez liste id które mają być dodane do podanej roli
                foreach(string userId in model.IdsToAdd ?? new string[] { })
                {
                    // znalezienie użytkownika po id następnie przypisanie użytkownikowi roli
                    AppUser user = await _userManager.FindByIdAsync(userId);
                    if(user != null)
                    {
                        result = await _userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            AddErrorsFromResult(result);
                    }
                }

                // pętla przechodząca przez liste id które mają być usunięte z podanej roli
                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    // znalezienie użytkownika po id następnie usunięcie użytkownikowi roli
                    AppUser user = await _userManager.FindByIdAsync(userId);
                    if(user != null)
                    {
                        result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            AddErrorsFromResult(result);
                    }
                }
            }

            if (ModelState.IsValid)
                return RedirectToAction(nameof(Index));
            else
                return await Edit(model.RoleId);
        }
        
        // Metoda dodająca errory do modelu
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}