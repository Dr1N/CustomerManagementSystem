using System.Collections.Generic;

namespace Domain.Entity
{
    public class Role
    {
        public Role()
        {
            RoleCustomer = new HashSet<RoleCustomer>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<RoleCustomer> RoleCustomer { get; set; }
    }
}
