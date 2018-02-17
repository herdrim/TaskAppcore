using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TaskAppCore.Models;
using TaskAppCore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TaskAppCore.Controllers
{
    [Authorize(Roles = "Admins")]
    public class AdminController : Controller
    {
        private UserManager<AppUser> userManager;
        // pola potrzebne do edycji usera
        private IUserValidator<AppUser> userValidator;
        private IPasswordValidator<AppUser> passwordValidator;
        private IPasswordHasher<AppUser> passwordHasher;

        // jesli chodzi o nazewnictwo to sklanialbym sie do nazywania w nawiązaniu do typu, np. teamRepository plus dobra praktyka jest dawanie przed prywatnymi polami "_"
        private ITeamRepository teamContext;

        public AdminController(UserManager<AppUser> usrManager, IUserValidator<AppUser> userValid, 
            IPasswordValidator<AppUser> passValid, IPasswordHasher<AppUser> passHash, ITeamRepository teamCtx)
        {
            userManager = usrManager;
            // inicjacja pól potrzebnych do edycji usera
            userValidator = userValid;
            passwordValidator = passValid;
            passwordHasher = passHash;
            teamContext = teamCtx;
        }
        // Dodając include mówisz entity frameworkowi, ze ma załączyć referencje do Tabeli Team i zaladować z niej dane - poczytaj o "lazy loading"
        public ViewResult Index() => View(userManager.Users.Include(x=>x.Team));

        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                    // Entity Framework zalatwi za ciebie cale powiazanie i utworzy Foreign Key'a
                    Team = teamContext.Teams.FirstOrDefault(t => t.TeamId == model.TeamId)
                };

                IdentityResult result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            // znajduje użytkownika po id, przypisuje go do user i sprawdza czy istnieje
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                // wynikiem DeleteAsync jest IdentityResult, które sprawdzamy czy się udało.
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    AddErrorsFromResult(result);
            }
            else
            {
                // jeżeli taki user nie istniał dodajem error
                ModelState.AddModelError("", "User Not Found");
            }

            return View("Index", userManager.Users);
        }

        // Metoda dodająca errory do modelu
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        public async Task<IActionResult> Edit(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);

            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string email, string password, int teamId)
        {
            // znalezienie usera po id
            AppUser user = await userManager.FindByIdAsync(id);

            if(user != null)
            {
                // Przypisanie do user.Email maila podanego z formularza
                user.Email = email;
                // walidacja usera z nowym mailem
                IdentityResult validEmail = await userValidator.ValidateAsync(userManager, user);
                if (!validEmail.Succeeded)
                    AddErrorsFromResult(validEmail);
                
                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(password))
                {
                    // walidacja hasła
                    validPass = await passwordValidator.ValidateAsync(userManager, user, password);
                    // jeżeli jest ok to hashowanie hasła
                    if (validPass.Succeeded)
                        user.PasswordHash = passwordHasher.HashPassword(user, password);
                    else
                        AddErrorsFromResult(validPass);
                }

                user.Team.TeamId = teamContext.Teams.Where(t => t.TeamId == teamId).FirstOrDefault().TeamId;

                if((validEmail.Succeeded && validPass == null) || 
                    (validEmail.Succeeded && password != string.Empty && validPass.Succeeded))
                {
                    // zapis usera ze zmienionymi danymi(update) - 
                    // dopóki ta metoda nie jest wywołana użytkownik nie jest zapisany do db
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        AddErrorsFromResult(result);
                }

            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }

            return View(user);
        }
    }
}