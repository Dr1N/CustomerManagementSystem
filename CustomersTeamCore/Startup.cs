using App.Mapper;
using App.Models;
using App.Service.Customers;
using App.Service.Login;
using App.Service.Password;
using DAL.Mapper;
using DAL.Models;
using DAL.Repository;
using DAL.SqlServer;
using Domain.Entity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CustomersTeamCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("CustomersDb")));

            services
                .AddAuthentication(
                    configure =>
                    {
                        configure.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        configure.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    })
                .AddCookie(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    config =>
                    {
                        config.Cookie.HttpOnly = true;
                        config.Cookie.Name = "Customers.303";
                        config.LoginPath = "/Account/Login";
                    }
            );

            services.AddAuthorization();

            services.AddControllersWithViews();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPasswordValidator, PasswordValidator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IMapper<Customer, CustomerDto>, CustomerDtoMapper>();
            services.AddScoped<IMapper<CustomerViewModel, Customer>, CustomerVmMapper>();
            services.AddScoped<IMapper<Customer, CustomerViewModel>, CustomerMapper>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IMapper<Role, RoleDto>, RoleDtoMapper>();

            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IRoleService, RoleService>();
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Customer/Error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
