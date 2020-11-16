using System;
using System.Linq;
using App.Models;
using DAL.Mapper;
using Domain.Entity;

namespace App.Mapper
{
    public class CustomerMapper : IMapper<Customer, CustomerViewModel>
    {
        public CustomerViewModel Create(Customer target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            return new CustomerViewModel
            {
                Id = target.Id,
                Login = target.Login,
                Active = !target.IsDisabled,
                Email = target.Email,
                FirstName = target.FirstName,
                LastName = target.LastName,
                Phone = target.PhoneNumber,
                RoleNames = target.RoleCustomer
                    .Select(r => r.Role.Name)
                    .ToList(),
                Created = target.Created.ToLocalTime().ToShortDateString(),
                CreatedByName = target.CreatedByCustomer.FirstName + " " + target.CreatedByCustomer.LastName,
                Update = target.Updated.ToLocalTime().ToShortDateString(),
                UpdateByName = target.UpdatedByCustomer.FirstName + " " + target.UpdatedByCustomer.LastName,
            };
        }

        public void Update(Customer from, CustomerViewModel to)
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
            to.Login = from.Login;
            to.Active = !from.IsDisabled;
            to.Email = from.Email;
            to.FirstName = from.FirstName;
            to.LastName = from.LastName;
            to.Phone = from.PhoneNumber;
            to.RoleNames = from.RoleCustomer
                .Select(r => r.Role.Name)
                .ToList();
            to.Created = from.Created.ToLocalTime().ToShortDateString();
            to.CreatedByName = from.CreatedByCustomer.FirstName + " " + from.CreatedByCustomer.LastName;
            to.Update = from.Updated.ToLocalTime().ToShortDateString();
            to.UpdateByName = from.UpdatedByCustomer.FirstName + " " + from.UpdatedByCustomer.LastName;
        }
    }
}
