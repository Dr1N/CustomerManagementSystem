using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Enums;
using App.Service.Password;
using DAL.Repository;
using Domain.Constants;
using Microsoft.Extensions.Logging;

namespace App.Service.Login
{
    public class LoginService : ILoginService
    {
        private readonly List<string> AvailableRoles = new List<string>()
        {
            RoleNames.Administrator,
            RoleNames.Operator,
            RoleNames.Manager,
        };

        private readonly ICustomerRepository customerRepository;
        private readonly IPasswordHasher passwordHasher;
        private readonly ILogger<LoginService> logger;

        public LoginService(
            ICustomerRepository customerRepository,
            IPasswordHasher passwordHasher,
            ILogger<LoginService> logger)
        {
            this.customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            this.passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LoginResult> LoginAsync(string login, string password)
        {
            try
            {
                var customer = await customerRepository
                    .FindByLoginAndPassword(login, passwordHasher.Hash(password))
                    .ConfigureAwait(false);

                if (customer == null)
                {
                    return LoginResult.Invalid;
                }

                if (customer.IsDisabled || customer.Deleted)
                {
                    return LoginResult.Disabled;
                }

                var hasRole = customer
                    .RoleCustomer
                    .Any(r => AvailableRoles.Contains(r.Role.Name));

                if (!hasRole)
                {
                    return LoginResult.Denied;
                }

                return LoginResult.Success;
            }
            catch (Exception ex)
            {
                logger.LogError($"Login error: {ex.Message}");
                return LoginResult.Error;
            }
        }
    }
}
