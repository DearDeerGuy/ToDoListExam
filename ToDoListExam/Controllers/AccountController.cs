using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToDoListExam.Models.ViewModels;

namespace ToDoListExam.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager
            )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public IActionResult Register() => View();
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await userManager.FindByEmailAsync(viewModel.Email);
                if(user == null)
                {
                    user = new IdentityUser()
                    {
                        UserName = viewModel.Login,
                        Email = viewModel.Email
                    };
                    IdentityResult result = await userManager.CreateAsync(user, viewModel.Password);
                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "ToDoList");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                        return View(viewModel);
                    }
                }
                return View(viewModel);
            }
            return View(viewModel);
        }

        public IActionResult Login(string? returnUrl)
        {
            LoginViewModel viewModel = new LoginViewModel() { ReturnUrl = returnUrl };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser? user = await userManager.FindByEmailAsync(viewModel.Email);
                if (user != null)
                {
                    var result = await signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(viewModel.ReturnUrl) && Url.IsLocalUrl(viewModel.ReturnUrl))
                        {
                            return Redirect(viewModel.ReturnUrl);
                        }
                        else return RedirectToAction("Index", "ToDoList");
                    }
                    ModelState.AddModelError(string.Empty, "Incorrect login and/or password");
                }
            }
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Logout(string? returnUrl)
        {
            await signInManager.SignOutAsync();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else return RedirectToAction("Login", "Account");
        }
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", Url.Action("GoogleResponse"));
            return new ChallengeResult("Google", properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse(string? returnUrl)
        {
            ExternalLoginInfo? externalLoginInfo = await signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
                return RedirectToAction("Login");
            var signInResult = await signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey, false);
            string?[] userInfo = new[] {
            externalLoginInfo.Principal?.FindFirst(ClaimTypes.Name)?.Value,
            externalLoginInfo.Principal?.FindFirst(ClaimTypes.Email)?.Value};
            if (signInResult.Succeeded)
                return RedirectToAction("Index", "ToDoList");
            IdentityUser? user = await userManager.FindByEmailAsync(userInfo[0]);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = externalLoginInfo.Principal?.FindFirst(ClaimTypes.Name)?.Value,
                    Email = externalLoginInfo.Principal?.FindFirst(ClaimTypes.Email)?.Value
                };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return RedirectToAction("AccessDenied");
            }
            var addLoginRes = await userManager.AddLoginAsync(user, externalLoginInfo);
            if (addLoginRes.Succeeded)
            {
                await signInManager.SignInAsync(user, false);
                //return View(userInfo);
                return RedirectToAction("Index", "ToDoList");
            }
            return RedirectToAction("AccessDenied");
        }
        public IActionResult AccessDenied() => View();
        [AllowAnonymous]
        public IActionResult GitHubLogin()
        {
            var authProperties = signInManager.ConfigureExternalAuthenticationProperties("GitHub", Url.Action("GoogleResponse"));
            return new ChallengeResult("GitHub", authProperties);
        }
    }
}
