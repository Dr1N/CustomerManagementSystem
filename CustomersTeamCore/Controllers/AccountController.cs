using System;
using System.Threading.Tasks;
using App.Constants;
using App.Enums;
using App.Models;
using App.Service.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CustomersTeamCore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILoginService loginService;
        private readonly IAuthService authService;
        private readonly ILogger<AccountController> logger;

        public AccountController(
            IAuthService authService,
            ILoginService loginService,
            ILogger<AccountController> logger)
        {
            this.loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Login()
        {
            try
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("List", "Customer");
                }

                return View(new LoginViewModel());
            }
            catch (Exception ex)
            {
                logger.LogError($"Login error: {ex.Message}");
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginResult = await loginService
                        .LoginAsync(loginViewModel.Login, loginViewModel.Password)
                        .ConfigureAwait(false);

                    switch (loginResult)
                    {
                        case LoginResult.Success:
                            await authService
                                .AuthAsync(loginViewModel.Login)
                                .ConfigureAwait(false);

                            return RedirectToAction("List", "Customer");
                        case LoginResult.Invalid:
                            ModelState.AddModelError(string.Empty, Messages.InvalidLoginPassword);
                            break;
                        case LoginResult.Disabled:
                            ModelState.AddModelError(string.Empty, Messages.DiabledAccount);
                            break;
                        case LoginResult.Denied:
                            ModelState.AddModelError(string.Empty, Messages.AccessDenied);
                            break;
                        case LoginResult.Error:
                            ModelState.AddModelError(string.Empty, Messages.ServerError);
                            break;
                    }
                }

                return View(loginViewModel);
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Login error: {ex.Message}");
                throw;
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await authService
                    .Logout()
                    .ConfigureAwait(false);

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                logger.LogError($"Logout error: {ex.Message}");
                throw;
            }
        }

        [Authorize]
        public IActionResult AccessDenied(string returnUrl)
        {
            try
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                logger.LogError($"Access denied error: {ex.Message}");
                throw;
            }
        }
    }
}
