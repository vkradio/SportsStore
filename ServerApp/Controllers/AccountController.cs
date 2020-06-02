using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Controllers
{
    public class AccountController : Controller
    {
        readonly UserManager<IdentityUser> userManager;
        readonly SignInManager<IdentityUser> signInManager;

        async Task<bool> DoLogin(LoginViewModel creds)
        {
            var user = await userManager.FindByNameAsync(creds.Name).ConfigureAwait(false);
            if (user != null)
            {
                await signInManager.SignOutAsync().ConfigureAwait(false);
                Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, creds.Password, false, false).ConfigureAwait(false);
                return result.Succeeded;
            }
            return false;
        }

        public AccountController(UserManager<IdentityUser> userMgr, SignInManager<IdentityUser> signInMgr) =>
            (userManager, signInManager) = (userMgr, signInMgr);

        [HttpGet]
        [SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "Uri is not working here")]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.returnUrl = returnUrl ?? null;
            return View();
        }

        [HttpPost]
        [SuppressMessage("Design", "CA1054:Uri parameters should not be strings", Justification = "Uri is not working here")]
        public async Task<IActionResult> Login(LoginViewModel creds, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                Guard.Against.Null(creds, nameof(creds));

                if (await DoLogin(creds).ConfigureAwait(false))
                    return Redirect(returnUrl ?? "/");
                else
                    ModelState.AddModelError(string.Empty, "Invalid username or password");
            }
            return View(creds);
        }
    }

    public class LoginViewModel
    {
        public string Name { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
