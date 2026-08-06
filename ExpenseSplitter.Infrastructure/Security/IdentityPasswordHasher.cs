using ExpenseSplitter.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using ExpenseSplitter.Application.Interfaces.Authentication;


namespace ExpenseSplitter.Infrastructure.Security
{
    public class IdentityPasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<User> _passwordHasher = new();

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null!, password);
        }

        public bool VerifyPassword(string hash, string password)
        {
            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(null!, hash, password);

            return result != PasswordVerificationResult.Failed;
        }
    }
}
