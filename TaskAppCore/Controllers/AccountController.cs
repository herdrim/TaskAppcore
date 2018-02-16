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
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;

        public AccountController(UserManager<AppUser> usrManager, SignInManager<AppUser> signManager)
        {
            userManager = usrManager;
            signInManager = signManager;
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
                AppUser user = await userManager.FindByEmailAsync(details.Email);
                if(user != null)
                {
                    // anulacja obecnej sesji użytkownika jeśli była(wylogowanie)
                    await signInManager.SignOutAsync();
                    // Zalogowanie
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, details.Password, false, false);

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
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied() => View();
    }
}