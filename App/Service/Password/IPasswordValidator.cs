using System.Collections.Generic;

namespace App.Service.Password
{
    public interface IPasswordValidator
    {
        IEnumerable<string> Errors(string password);
    }
}