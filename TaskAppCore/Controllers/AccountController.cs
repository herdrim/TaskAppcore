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

        public AccountController(UserManager<AppUser> usrManager, SignInManager<AppUser> signManager)
        {
            _userManager = usrManager;
            _signInManager = signManager;
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