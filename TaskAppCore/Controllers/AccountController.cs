using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaskAppCore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using TaskAppCore.Models;

namespace TaskAppCore.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private IUserRepository _userRepository;

        public AccountController(UserManager<AppUser> usrManager, SignInManager<AppUser> signManager, IUserRepository userRepository)
        {
            _userManager = usrManager;
            _signInManager = signManager;
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserModel details, string returnUrl)
        {
            if(ModelState.IsValid)
            {
                // wyszukujemy użytkownika po emailu
                AppUser user = await _userManager.FindByEmailAsync(details.Email);
                if(user != null)
                {
                    // anulacja obecnej sesji użytkownika jeśli była(wylogowanie)
                    await _signInManager.SignOutAsync();
                    // Zalogowanie
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, details.Password, false, false);

                    // (returnUrl ?? "/") jest jak -> (returnUrl != null ? returnUrl : "/")
                    if (result.Succeeded)
                        return Redirect(returnUrl ?? "/");
                }

                ModelState.AddModelError(nameof(LoginUserModel.Email), "Invalid user or password");
            }
            return View(details);
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserModel model)
        {
            if (ModelState.IsValid)
            {
                bool isError = false;

                AppUser email = _userRepository.Users.FirstOrDefault(x => x.Email == model.Email);
                if (email != null)
                {
                    ModelState.AddModelError(nameof(RegisterUserModel.Email), "E-mail is already taken");
                    isError = true;
                }
                AppUser userName = _userRepository.Users.FirstOrDefault(x => x.NormalizedUserName == model.UserName.ToUpper());
                if (userName != null)
                {
                    ModelState.AddModelError(nameof(RegisterUserModel.UserName), "UserName is already taken");
                    isError = true;
                }
                if (model.Password.Length < 4)
                {
                    ModelState.AddModelError(nameof(RegisterUserModel.Password), "Password is too short");
                    isError = true;
                }

                if(!isError)
                {
                    AppUser user = new AppUser
                    {
                        Email = model.Email,
                        UserName = model.UserName
                    };

                    IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                        return RedirectToAction("Login");
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied() => View();
    }
}