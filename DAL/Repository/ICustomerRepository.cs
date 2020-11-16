using System.Linq;
using System.Threading.Tasks;
using Domain.Entity;

namespace DAL.Repository
{
    public interface ICustomerRepository
    {
        void Add(Customer customer);

        IQueryable<Customer> Customers();

        Task<Customer> FindByIdAsync(int id);

        Task<Customer> FindByLogin(string login);

        Task<Customer> FindByLoginAndPassword(string login, string password);

        Task SaveChangesAsync();
    }
}
