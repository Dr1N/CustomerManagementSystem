using System.Threading.Tasks;

namespace App.Service.Login
{
    public interface IAuthService
    {
        Task AuthAsync(string login);

        Task Logout();
    }
}
