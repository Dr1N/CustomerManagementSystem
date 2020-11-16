using System;
using System.Linq;
using System.Threading.Tasks;
using App.Constants;
using App.Models;
using App.Service.Customers;
using App.Service.Password;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace CustomersTeamCore.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService customerService;
        private readonly IRoleService roleService;
        private readonly IPasswordValidator passwordValidator;
        private readonly ILogger<CustomerController> logger;

        public CustomerController(
            ICustomerService customerService,
            IRoleService roleService,
            IPasswordValidator passwordValidator,
            ILogger<CustomerController> logger)
        {
            this.customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            this.roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            this.passwordValidator = passwordValidator ?? throw new ArgumentNullException(nameof(passwordValidator));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Authorize]
        public async Task<IActionResult> List()
        {
            try
            {
                var currentUser = await customerService
                    .FindByLoginAsync(HttpContext.User.Identity.Name)
                    .ConfigureAwait(false);

                var canEdit = currentUser
                    .RoleCustomer
                    .Count(r => r.Role.Name == RoleNames.Administrator || r.Role.Name == RoleNames.Manager);

                ViewBag.CanEdit = canEdit != 0 ? "true" : "false";

                return View();
            }
            catch (Exception ex)
            {
                logger.LogError($"Customers list error: {ex.Message}");
                throw;
            }
        }

        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> Create()
        {
            try
            {
                await PrepareRoleListAsync().ConfigureAwait(false);

                return View(new CustomerViewModel());
            }
            catch (Exception ex)
            {
                logger.LogError($"Customer creating error: {ex.Message}");
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> Create(CustomerViewModel customerViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginExists = await customerService
                        .HasLoginAsync(customerViewModel)
                        .ConfigureAwait(false);

                    if (loginExists)
                    {
                        ModelState.AddModelError(
                            nameof(CustomerViewModel.Login),
                            $"{customerViewModel.Login} is already in use");

                        await PrepareRoleListAsync().ConfigureAwait(false);

                        return View(customerViewModel);
                    }

                    await customerService
                        .CreateAsync(customerViewModel, HttpContext.User.Identity.Name)
                        .ConfigureAwait(false);

                    return RedirectToAction("List");
                }

                await PrepareRoleListAsync().ConfigureAwait(false);

                return View(customerViewModel);
            }
            catch (Exception ex)
            {
                logger.LogError($"Customer creating error: {ex.Message}");
                throw;
            }
        }

        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                if (id == CommonConstants.SuperAdminId)
                {
                    logger.LogWarning("Cannot edit super admin account");
                    return View("Info", "Sorry this is a super admin account");
                }

                var customerViewModel = await customerService
                    .CreateViewModelByIdAsync(id)
                    .ConfigureAwait(false);

                if (customerViewModel == null)
                {
                    logger.LogError($"Not found customer database record. Id: {id}");
                    return NotFound();
                }

                await PrepareRoleListAsync().ConfigureAwait(false);

                return View(customerViewModel);
            }
            catch (Exception ex)
            {
                logger.LogError($"Customer editing error: {ex.Message}");
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> Edit(CustomerViewModel customerViewModel)
        {
            try
            {
                if (customerViewModel.Id == CommonConstants.SuperAdminId)
                {
                    logger.LogWarning("Cannot edit super admin account");
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var hasLogin = await customerService
                        .HasLoginAsync(customerViewModel)
                        .ConfigureAwait(false);

                    if (hasLogin)
                    {
                        ModelState.AddModelError(
                            nameof(CustomerViewModel.Login),
                            $"{customerViewModel.Login} is already in use");

                        await PrepareRoleListAsync().ConfigureAwait(false);

                        return View(customerViewModel);
                    }

                    await customerService
                        .UpdateAsync(customerViewModel, HttpContext.User.Identity.Name)
                        .ConfigureAwait(false);

                    return RedirectToAction("List");
                }

                await PrepareRoleListAsync().ConfigureAwait(false);

                return View(customerViewModel);
            }
            catch (Exception ex)
            {
                logger.LogError($"Customer editing error: {ex.Message}");
                throw;
            }
        }

        public IActionResult Error()
        {
            return View();
        }

        [AcceptVerbs("GET", "POST")]
        [Authorize(Roles = "Administrator,Manager")]
        public IActionResult VerifyPassword(string password)
        {
            try
            {
                var errors = passwordValidator.Errors(password);

                if (errors.Any())
                {
                    return Json($"{string.Join(" ", errors) }");
                }

                return Json(true);
            }
            catch (Exception ex)
            {
                logger.LogError($"Verify password error: {ex.Message}");
                return Json("Server error");
            }
        }

        private async Task PrepareRoleListAsync()
        {
            var availableRoleNames = await roleService
               .RoleNamesAsync()
               .ConfigureAwait(false);

            ViewBag.RoleNames = availableRoleNames
                .Select(r => new SelectListItem { Value = r, Text = r })
                .ToList();
        }
    }
}
