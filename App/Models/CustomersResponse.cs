using System.Collections.Generic;
using DAL.Models;

namespace App.Models
{
    public class CustomersResponse
    {
        public IEnumerable<CustomerDto> Customers { get; set; }

        public int TotalCustomers { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }
    }
}
