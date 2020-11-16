using System;
using System.Collections.Generic;

namespace Domain.Entity
{
    public class Customer
    {
        public Customer()
        {
            CreatedCustomers = new HashSet<Customer>();
            UpdatedCustomers = new HashSet<Customer>();
            RoleCustomer = new HashSet<RoleCustomer>();
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public bool IsDisabled { get; set; }

        public DateTime Created { get; set; }

        public int CreatedBy { get; set; }

        public DateTime Updated { get; set; }

        public int UpdatedBy { get; set; }

        public bool Deleted { get; set; }

        public virtual Customer CreatedByCustomer { get; set; }

        public virtual Customer UpdatedByCustomer { get; set; }

        public virtual ICollection<Customer> CreatedCustomers { get; set; }

        public virtual ICollection<Customer> UpdatedCustomers { get; set; }

        public virtual ICollection<RoleCustomer> RoleCustomer { get; set; }
    }
}
