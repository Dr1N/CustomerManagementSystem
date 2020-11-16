using System;
using System.IO;
using System.Text;

namespace Scaffold
{
    internal static class Program
    {
        private const string fileName = "sql.sql";

        private static void Main()
        {
            try
            {
                GenerateScript(55, 2, 6);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void GenerateScript(int count, int start, int id)
        {
            var sbCustomers = new StringBuilder();
            var sbRoles = new StringBuilder();
            var ph = new PasswordHasher();

            Console.WriteLine("Generate SQL");

            for (int i = 0; i < count; i++)
            {
                var hash = ph.Encrypt($"Customer{i + start}@");
                sbCustomers.AppendFormat(
                    "INSERT INTO [dbo].[customers] ([FirstName], [LastName], [Email], [PhoneNumber], [Login], [Password], [IsDisabled], [Created], [CreatedBy], [Updated], [UpdatedBy], [Deleted]) " +
                    $"VALUES (N'Customer{i + start}', N'Customer{i + start}', N'customer{i + start}@gmail.com', N'1234567890', N'customer{i + start}', N'{hash}', 0, N'2020-11-03 09:42:40', 1, N'2020-11-03 09:42:40', 1, 0)");
                sbCustomers.AppendLine();

                sbRoles.AppendFormat($"INSERT INTO[dbo].[role_customer]([CustomerId], [RoleId]) VALUES({i + id}, 2)");
                sbRoles.AppendLine();
            }

            Console.WriteLine("Write to file");

            File.WriteAllText(fileName, sbCustomers.ToString());
            File.AppendAllText(fileName, sbRoles.ToString());
        }
    }
}
