using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.SqlServer;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DbSet<Role> dbSet;

        public RoleRepository(ApplicationContext db)
        {
            this.dbSet = db.Roles ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<IEnumerable<Role>> FindAllAsync()
        {
            return await dbSet
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<Role> FindByNameAsync(string name)
        {
            return await dbSet
                .FirstOrDefaultAsync(r => r.Name == name)
                .ConfigureAwait(false);
        }
    }
}
