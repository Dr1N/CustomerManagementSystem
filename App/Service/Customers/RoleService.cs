using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Repository;

namespace App.Service.Customers
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            this.roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        }

        public async Task<IEnumerable<string>> RoleNamesAsync()
        {
            return (await roleRepository
                    .FindAllAsync()
                    .ConfigureAwait(false))
                .Select(r => r.Name);
        }
    }
}
