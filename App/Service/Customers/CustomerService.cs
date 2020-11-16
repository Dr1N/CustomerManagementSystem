using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Models;
using DAL.Mapper;
using DAL.Models;
using DAL.Repository;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.Service.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository customerRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IMapper<CustomerViewModel, Customer> customerVmMapper;
        private readonly IMapper<Customer, CustomerViewModel> customerMapper;
        private readonly IMapper<Customer, CustomerDto> customerDtoMapper;
        private readonly ILogger<CustomerService> logger;

        public CustomerService(
            ICustomerRepository customerRepository,
            IRoleRepository roleRepository,
            IMapper<CustomerViewModel, Customer> customerVmMapper,
            IMapper<Customer, CustomerViewModel> customerMapper,
            IMapper<Customer, CustomerDto> customerDtoMapper,
            ILogger<CustomerService> logger)
        {
            this.customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            this.roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            this.customerVmMapper = customerVmMapper ?? throw new ArgumentNullException(nameof(customerVmMapper));
            this.customerMapper = customerMapper ?? throw new ArgumentNullException(nameof(customerMapper));
            this.customerDtoMapper = customerDtoMapper ?? throw new ArgumentNullException(nameof(customerDtoMapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task CreateAsync(CustomerViewModel customerViewModel, string currentUser)
        {
            if (customerViewModel == null)
            {
                throw new ArgumentNullException(nameof(customerViewModel));
            }

            var creator = await CurrentUserAsync(currentUser).ConfigureAwait(false);

            if (creator == null)
            {
                string messsage = $"Not found current user in data base. Login: {currentUser}";
                logger.LogError(messsage);
                throw new InvalidOperationException(messsage);
            }

            var customer = customerVmMapper.Create(customerViewModel);
            customer.Created = DateTime.UtcNow;
            customer.CreatedByCustomer = creator;
            customer.Updated = DateTime.UtcNow;
            customer.UpdatedByCustomer = creator;

            var customerRoles = await GetCustomerRolesAsync(customerViewModel.RoleNames, customer)
                .ConfigureAwait(false);

            customerRoles.ForEach(rc => customer.RoleCustomer.Add(rc));

            customerRepository.Add(customer);

            await customerRepository
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }

        public async Task UpdateAsync(CustomerViewModel customerViewModel, string currentUser)
        {
            var updater = await CurrentUserAsync(currentUser).ConfigureAwait(false);

            if (updater == null)
            {
                string messsage = $"Not found current user in data base. Login: {currentUser}";
                logger.LogError(messsage);
                throw new InvalidOperationException(messsage);
            }

            var customer = await customerRepository
                .Customers()
                .FirstOrDefaultAsync(c => c.Id == customerViewModel.Id && !c.Deleted)
                .ConfigureAwait(false);

            if (customer == null)
            {
                string messsage = $"Not found customer in data base. Id: {customerViewModel.Id}";
                logger.LogError(messsage);
                throw new InvalidOperationException(messsage);
            }

            customerVmMapper.Update(customerViewModel, customer);
            customer.Updated = DateTime.UtcNow;
            customer.UpdatedByCustomer = updater;

            customer.RoleCustomer.Clear();

            var customerRoles = await GetCustomerRolesAsync(customerViewModel.RoleNames, customer)
                .ConfigureAwait(false);

            customerRoles.ForEach(rc => customer.RoleCustomer.Add(rc));

            await customerRepository
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(int id, string currentUser)
        {
            var updater = await CurrentUserAsync(currentUser)
                .ConfigureAwait(false);

            if (updater == null)
            {
                string messsage = $"Not found current user in data base. Login: {currentUser}";
                logger.LogError(messsage);
                throw new InvalidOperationException(messsage);
            }

            var customer = await customerRepository
                .FindByIdAsync(id)
                .ConfigureAwait(false);

            if (customer == null)
            {
                string messsage = $"Not found customer in data base. Id: {id}";
                logger.LogError(messsage);
                throw new InvalidOperationException(messsage);
            }

            customer.Deleted = true;

            await customerRepository
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }

        public async Task<CustomerViewModel> CreateViewModelByIdAsync(int id)
        {
            var customer = await customerRepository
                .Customers()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id && !c.Deleted)
                .ConfigureAwait(false);

            if (customer == null)
            {
                string messsage = $"Not found customer in data base. Id: {id}";
                logger.LogError(messsage);
                throw new InvalidOperationException(messsage);
            }

            return customerMapper.Create(customer);
        }

        public async Task<bool> HasLoginAsync(CustomerViewModel customerViewModel)
        {
            var count = await customerRepository
                .Customers()
                .CountAsync(c => c.Login == customerViewModel.Login && c.Id != customerViewModel.Id)
                .ConfigureAwait(false);

            return count > 0;
        }

        public async Task<Customer> FindByLoginAsync(string login)
        {
            return await customerRepository
                .Customers()
                .FirstOrDefaultAsync(c => c.Login == login && !c.Deleted)
                .ConfigureAwait(false);
        }

        public async Task<int> CustomersCountTotalAsync()
        {
            return await customerRepository
                .Customers()
                .Where(c => !c.Deleted)
                .CountAsync()
                .ConfigureAwait(false);
        }

        public async Task<int> CustomersCountAsync(string search)
        {
            return await customerRepository
                .Customers()
                .Where(c => !c.Deleted)
                .Where(c => c.Login.Contains(search)
                        || c.FirstName.Contains(search)
                        || c.LastName.Contains(search)
                        || c.PhoneNumber.Contains(search)
                        || c.Email.Contains(search)
                        || c.RoleCustomer.Any(r => r.Role.Name.Contains(search)))
                .CountAsync()
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<CustomerDto>> GetCustomersDtoAsync(
            string orderby,
            string sortDirection,
            string search,
            int skip,
            int take)
        {
            if (skip < 0)
            {
                throw new ArgumentException(nameof(skip));
            }

            if (take < 0)
            {
                throw new ArgumentException(nameof(take));
            }

            var result = new List<CustomerDto>();

            var customers = customerRepository
                .Customers()
                .Where(c => !c.Deleted)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(orderby) && !string.IsNullOrEmpty(sortDirection))
            {
                customers = SetOrder(customers, orderby, sortDirection);
            }

            if (!string.IsNullOrEmpty(search))
            {
                customers = customers
                    .Where(c => c.Login.Contains(search)
                        || c.FirstName.Contains(search)
                        || c.LastName.Contains(search)
                        || c.PhoneNumber.Contains(search)
                        || c.Email.Contains(search)
                        || c.RoleCustomer.Any(r => r.Role.Name.Contains(search)));
            }

            var customersResult = await customers
                .Skip(skip)
                .Take(take)
                .ToListAsync()
                .ConfigureAwait(false);

            customersResult.ForEach(c => result.Add(customerDtoMapper.Create(c)));

            return result;
        }

        private async Task<Customer> CurrentUserAsync(string login)
        {
            return await customerRepository
                .Customers()
                .FirstOrDefaultAsync(c => c.Login == login && !c.Deleted)
                .ConfigureAwait(false);
        }

        private async Task<List<RoleCustomer>> GetCustomerRolesAsync(
            IEnumerable<string> roleNames,
            Customer customer)
        {
            var roles = await FindRolesByNamesAsync(roleNames)
                .ConfigureAwait(false);

            if (roles.Count == 0)
            {
                const string message = "Can't create customer. No available roles";
                logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            return roles.ConvertAll(r => new RoleCustomer() { Role = r, Customer = customer });
        }

        private async Task<List<Role>> FindRolesByNamesAsync(IEnumerable<string> roleNames)
        {
            var result = new List<Role>();

            foreach (var roleName in roleNames)
            {
                var role = await roleRepository
                    .FindByNameAsync(roleName)
                    .ConfigureAwait(false);

                if (role != null)
                {
                    result.Add(role);
                }
            }

            return result;
        }

        private IQueryable<Customer> SetOrder(IQueryable<Customer> customers, string orderby, string sortDirection)
        {
            switch (orderby)
            {
                case "loginSort":
                    if (string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase))
                    {
                        customers = customers.OrderBy(c => c.Login);
                    }
                    else if (string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase))
                    {
                        customers = customers.OrderByDescending(c => c.Login);
                    }
                    break;
                case "nameSort":
                    if (string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase))
                    {
                        customers = customers.OrderBy(c => c.FirstName);
                    }
                    else if (string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase))
                    {
                        customers = customers.OrderByDescending(c => c.FirstName);
                    }
                    break;
                case "emailSort":
                    if (string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase))
                    {
                        customers = customers.OrderBy(c => c.Email);
                    }
                    else if (string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase))
                    {
                        customers = customers.OrderByDescending(c => c.Email);
                    }
                    break;
                case "phoneSort":
                    if (string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase))
                    {
                        customers = customers.OrderBy(c => c.PhoneNumber);
                    }
                    else if (string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase))
                    {
                        customers = customers.OrderByDescending(c => c.PhoneNumber);
                    }
                    break;
            }

            return customers;
        }
    }
}
