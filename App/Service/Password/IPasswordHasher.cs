namespace App.Service.Password
{
    public interface IPasswordHasher
    {
        string Hash(string password);
    }
}