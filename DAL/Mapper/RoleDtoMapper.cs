using System;
using DAL.Models;
using Domain.Entity;

namespace DAL.Mapper
{
    public class RoleDtoMapper : IMapper<Role, RoleDto>
    {
        public RoleDto Create(Role target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            return new RoleDto
            {
                Id = target.Id,
                Name = target.Name,
            };
        }

        public void Update(Role from, RoleDto to)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            to.Id = from.Id;
            to.Name = from.Name;
        }
    }
}
