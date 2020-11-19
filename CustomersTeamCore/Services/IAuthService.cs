using System.Threading.Tasks;

namespace CustomersTeamCore.Services
{
    public interface IAuthService
    {
        Task AuthAsync(string login);

        Task Logout();
    }
}
