using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Constants;
using App.Models;
using App.Service.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CustomersTeamCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private const string RecordPerPage = "RecordPerPage";

        private readonly ICustomerService customerService;
        private readonly ILogger<CustomersController> logger;

        private readonly int customersPerPage = 10;

        public CustomersController(
            ICustomerService customerService,
            IConfiguration configuration,
            ILogger<CustomersController> logger)
        {
            this.customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            try
            {
                customersPerPage = (int)configuration.GetValue(typeof(int), RecordPerPage);
                if (customersPerPage <= 5)
                {
                    customersPerPage = 5;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        [HttpGet("{loginSort}/{nameSort}/{emailSort}/{phoneSort}/{s}/{page:int}"), Route("fetch")]
        [Authorize(Roles = "Administrator,Operator,Manager")]
        public async Task<IActionResult> LoadCustomers(
            [FromQuery] string loginSort,
            [FromQuery] string nameSort,
            [FromQuery] string emailSort,
            [FromQuery] string phoneSort,
            [FromQuery] string s,
            [FromQuery] int page)
        {
            if (page <= 0)
            {
                logger.LogError($"Page is less or equal to zero: {page}");
            }

            try
            {
                var sortConditions = GetSortConditions(loginSort, nameSort, emailSort, phoneSort);
                var skip = (page - 1) * customersPerPage;

                var customers = await customerService
                    .GetCustomersDtoAsync(sortConditions.Item1, sortConditions.Item2, s, skip, customersPerPage)
                    .ConfigureAwait(false);

                var totalCustomers = await customerService
                    .CustomersCountTotalAsync()
                    .ConfigureAwait(false);

                var totalFilteredCustomers = totalCustomers;

                if (!string.IsNullOrEmpty(s))
                {
                    totalFilteredCustomers = await customerService
                        .CustomersCountAsync(s)
                        .ConfigureAwait(false);
                }

                var response = new CustomersResponse
                {
                    Customers = customers,
                    TotalCustomers = totalCustomers,
                    Page = page,
                    TotalPages = (int)Math.Ceiling((double)totalFilteredCustomers / customersPerPage),
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError($"Load customers error: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("id:int"), Route("delete")]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> DeleteCustomer([FromBody] int id)
        {
            try
            {
                var message = string.Empty;
                var success = true;

                if (id == CommonConstants.SuperAdminId)
                {
                    const string mess = "You cannot delete super admin record";
                    logger.LogError(mess);
                    success = false;
                    message = mess;

                    return BadRequest(new { success, message });
                }

                var currentUser = await customerService
                    .FindByLoginAsync(HttpContext.User.Identity.Name)
                    .ConfigureAwait(false);

                if (currentUser == null)
                {
                    logger.LogError("Not found current user data base record");
                    success = false;

                    return BadRequest("Not found current user data base record");
                }

                if (currentUser.Id == id)
                {
                    const string mess = "You cannot delete your own record";
                    logger.LogError(mess);
                    success = false;
                    message = mess;

                    return BadRequest(new { success, message });
                }

                await customerService
                    .DeleteAsync(id, HttpContext.User.Identity.Name)
                    .ConfigureAwait(false);

                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                logger.LogError($"Deleting error: {ex.Message}");
                return BadRequest(ex);
            }
        }

        private Tuple<string, string> GetSortConditions(
            string loginSort,
            string nameSort,
            string emailSort,
            string phoneSort)
        {
            var sorts = new Dictionary<string, string>
            {
                { nameof(loginSort), loginSort },
                { nameof(nameSort), nameSort },
                { nameof(emailSort), emailSort },
                { nameof(phoneSort), phoneSort },
            };

            var currentSort = sorts.FirstOrDefault(s =>
                !string.Equals(SortingConstants.None, s.Value, StringComparison.OrdinalIgnoreCase));

            var orderby = currentSort.Key ?? string.Empty;
            var sortDirection = currentSort.Value ?? string.Empty;

            return Tuple.Create(orderby, sortDirection);
        }
    }
}
