using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DAL.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace App.Service.Login
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ICustomerRepository repository;

        public AuthService(ICustomerRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task AuthAsync(string user)
        {
            var customer = await repository
                .FindByLogin(user)
                .ConfigureAwait(false);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, customer.Login)
            };

            customer
                .RoleCustomer
                .ToList()
                .ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r.Role.Name)));

            var identity = new ClaimsIdentity(claims, "ApplicationCookie");

            var principal = new ClaimsPrincipal(identity);

            await httpContextAccessor.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal)
                .ConfigureAwait(false);
        }

        public async Task Logout()
        {
            await httpContextAccessor.HttpContext
                .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)
                .ConfigureAwait(false);
        }
    }
}
