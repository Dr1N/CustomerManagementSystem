using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Service.Customers
{
    public interface IRoleService
    {
        Task<IEnumerable<string>> RoleNamesAsync();
    }
}