using System.Threading.Tasks;
using App.Enums;

namespace App.Service.Login
{
    public interface ILoginService
    {
        Task<LoginResult> LoginAsync(string login, string password);
    }
}
