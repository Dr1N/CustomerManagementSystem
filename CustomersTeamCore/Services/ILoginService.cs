using System.Threading.Tasks;
using App.Enums;

namespace CustomersTeamCore.Services
{
    public interface ILoginService
    {
        Task<LoginResult> LoginAsync(string login, string password);
    }
}
