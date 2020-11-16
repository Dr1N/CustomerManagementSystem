namespace DAL.Models
{
    public class CustomerDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Login { get; set; }

        public bool Active { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
