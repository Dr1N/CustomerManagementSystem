using System;
using System.Linq;
using System.Threading.Tasks;
using DAL.SqlServer;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationContext context;
        private readonly DbSet<Customer> customers;

        public CustomerRepository(ApplicationContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            customers = context.Customers ?? throw new ArgumentNullException(nameof(context));
        }

        public void Add(Customer customer)
        {
            customers.Add(customer);
        }

        // TODO: Bad method. Refactor him
        public IQueryable<Customer> Customers()
        {
            return CustomersWithRelations();
        }

        public async Task<Customer> FindByIdAsync(int id)
        {
            return await CustomersWithRelations()
                .FirstOrDefaultAsync(c => c.Id == id)
                .ConfigureAwait(false);
        }

        public async Task<Customer> FindByLogin(string login)
        {
            return await CustomersWithRelations()
                .FirstOrDefaultAsync(c => c.Login == login)
                .ConfigureAwait(false);
        }

        public async Task<Customer> FindByLoginAndPassword(string login, string password)
        {
            return await CustomersWithRelations()
                .FirstOrDefaultAsync(c => c.Login == login && c.Password == password)
                .ConfigureAwait(false);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync().ConfigureAwait(false);
        }

        private IQueryable<Customer> CustomersWithRelations()
        {
            return customers
                .Include(c => c.RoleCustomer).ThenInclude(rc => rc.Role)
                .Include(c => c.CreatedByCustomer)
                .Include(c => c.UpdatedByCustomer);
        }
    }
}
