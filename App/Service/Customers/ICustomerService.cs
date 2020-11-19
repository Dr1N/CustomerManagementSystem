using System.Collections.Generic;
using System.Threading.Tasks;
using App.Models;
using DAL.Models;
using Domain.Entity;

namespace App.Service.Customers
{
    public interface ICustomerService
    {
        Task CreateAsync(CustomerViewModel customerViewModel, string currentUser);

        Task UpdateAsync(CustomerViewModel customerViewModel, string currentUser);

        Task DeleteAsync(int id, string currentUser);

        Task<CustomerViewModel> CreateViewModelByIdAsync(int id);

        Task<bool> HasLoginAsync(CustomerViewModel customerViewModel);

        Task<Customer> FindByLoginAsync(string login);

        Task<int> CustomersCountTotalAsync();

        Task<int> CustomersCountAsync(string search);

        Task<IEnumerable<CustomerDto>> GetCustomersDtoAsync(string orderby, string sortDirection, string search, int skip, int take);
    }
}
