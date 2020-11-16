namespace Domain.Entity
{
    public class RoleCustomer
    {
        public int CustomerId { get; set; }

        public int RoleId { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Role Role { get; set; }
    }
}
