using System;
using DAL.Models;
using Domain.Entity;

namespace DAL.Mapper
{
    public class CustomerDtoMapper : IMapper<Customer, CustomerDto>
    {
        public CustomerDto Create(Customer target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            return new CustomerDto()
            {
                Id = target.Id,
                FirstName = target.FirstName,
                LastName = target.LastName,
                Email = target.Email,
                Phone = target.PhoneNumber,
                Login = target.Login,
                Active = !target.IsDisabled,
            };
        }

        public void Update(Customer from, CustomerDto to)
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
            to.FirstName = from.FirstName;
            to.LastName = from.LastName;
            to.Email = from.Email;
            to.Phone = from.PhoneNumber;
            to.Login = from.Login;
            to.Active = !from.IsDisabled;
        }
    }
}
