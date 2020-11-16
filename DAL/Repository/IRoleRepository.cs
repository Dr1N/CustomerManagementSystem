using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entity;

namespace DAL.Repository
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> FindAllAsync();

        Task<Role> FindByNameAsync(string name);
    }
}
