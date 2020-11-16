using System;
using App.Models;
using App.Service.Password;
using DAL.Mapper;
using Domain.Entity;

namespace App.Mapper
{
    public class CustomerVmMapper : IMapper<CustomerViewModel, Customer>
    {
        private readonly IPasswordHasher passwordHasher;

        public CustomerVmMapper(IPasswordHasher passwordHasher)
        {
            this.passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public Customer Create(CustomerViewModel target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            return new Customer
            {
                Login = target.Login,
                Password = passwordHasher.Hash(target.Password),
                FirstName = target.FirstName,
                LastName = target.LastName,
                PhoneNumber = target.Phone,
                Email = target.Email,
                IsDisabled = !target.Active,
            };
        }

        public void Update(CustomerViewModel from, Customer to)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            to.Login = from.Login;
            to.Password = passwordHasher.Hash(from.Password);
            to.FirstName = from.FirstName;
            to.LastName = from.LastName;
            to.PhoneNumber = from.Phone;
            to.Email = from.Email;
            to.IsDisabled = !from.Active;
        }
    }
}
