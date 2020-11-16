using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Service.Password
{
    public class PasswordValidator : IPasswordValidator
    {
        private const int MaxPasswordLength = 12;

        private const int MinPasswrodLength = 6;

        private readonly char[] SpecialSymbols = { '@', '#', '$', '%', '&', '.', '_' };

        public IEnumerable<string> Errors(string password)
        {
            var errors = new List<string>();

            if (password.Length < MinPasswrodLength || password.Length > MaxPasswordLength)
            {
                errors.Add("Password must be between 6 and 12 symbols length");
            }

            if (!password.Any(char.IsUpper))
            {
                errors.Add("Password must contain an uppercase character");
            }

            if (!password.Any(char.IsLower))
            {
                errors.Add("Password must contain an lowercase character");
            }

            if (!password.Any(char.IsDigit))
            {
                errors.Add("Password must contain an digit character");
            }

            if (!password.Any(c => SpecialSymbols.Contains(c)))
            {
                errors.Add($"Password must contain an special character from: {string.Join(' ', SpecialSymbols)}");
            }

            return errors;
        }
    }
}
