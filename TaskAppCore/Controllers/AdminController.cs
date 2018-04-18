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
        private UserManager<AppUser> _userManager;
        // pola potrzebne do edycji usera
        private IUserValidator<AppUser> _userValidator;
        private IPasswordValidator<AppUser> _passwordValidator;
        private IPasswordHasher<AppUser> _passwordHasher;

        private ITeamRepository _teamRepository;
        private IUserRepository _userRepository;

        public AdminController(UserManager<AppUser> usrManager, IUserValidator<AppUser> userValid, 
            IPasswordValidator<AppUser> passValid, IPasswordHasher<AppUser> passHash, ITeamRepository teamRepo, IUserRepository userRepository)
        {
            _userManager = usrManager;
            // inicjacja pól potrzebnych do edycji usera
            _userValidator = userValid;
            _passwordValidator = passValid;
            _passwordHasher = passHash;
            _teamRepository = teamRepo;
            _userRepository = userRepository;
        }
        // Dodając include mówisz entity frameworkowi, ze ma załączyć referencje do Tabeli Team i zaladować z niej dane - poczytaj o "lazy loading"
        public ViewResult Index() => View(_userManager.Users.Include(x => x.Team));

        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(UserAdminCreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.Name,
                    Email = model.Email
                };

                if (model.TeamId > 0)
                    user.TeamId = _teamRepository.Teams.FirstOrDefault(t => t.TeamId == model.TeamId).TeamId;

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);              

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
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // wynikiem DeleteAsync jest IdentityResult, które sprawdzamy czy się udało.
                IdentityResult result = await _userManager.DeleteAsync(user);
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

            return View("Index", _userManager.Users);
        }

        // Metoda dodająca errory do modelu
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        public async Task<IActionResult> Edit(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);

            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string email, string password, int teamId)
        {
            // znalezienie usera po id
            AppUser user = await _userManager.FindByIdAsync(id);

            if(user != null)
            {
                // Przypisanie do user.Email maila podanego z formularza
                user.Email = email;
                // walidacja usera z nowym mailem
                IdentityResult validEmail = await _userValidator.ValidateAsync(_userManager, user);
                if (!validEmail.Succeeded)
                    AddErrorsFromResult(validEmail);
                
                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(password))
                {
                    // walidacja hasła
                    validPass = await _passwordValidator.ValidateAsync(_userManager, user, password);
                    // jeżeli jest ok to hashowanie hasła
                    if (validPass.Succeeded)
                        user.PasswordHash = _passwordHasher.HashPassword(user, password);
                    else
                        AddErrorsFromResult(validPass);
                }

                if (_teamRepository.Teams.FirstOrDefault(t => t.TeamId == teamId) != null && _userRepository.Users.FirstOrDefault(u => u.Id == id).TeamId != teamId)
                {
                    user.TeamId = teamId;
                    _userRepository.Users.FirstOrDefault(u => u.Id == id).Tasks = null;
                }

                if((validEmail.Succeeded && validPass == null) || 
                    (validEmail.Succeeded && password != string.Empty && validPass.Succeeded))
                {
                    // zapis usera ze zmienionymi danymi(update) - 
                    // dopóki ta metoda nie jest wywołana użytkownik nie jest zapisany do db
                    IdentityResult result = await _userManager.UpdateAsync(user);
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