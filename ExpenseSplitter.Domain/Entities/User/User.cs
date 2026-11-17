using ExpenseSplitter.Domain.Enums;

namespace ExpenseSplitter.Domain.Entities.User
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string? PasswordHash { get; set; }
        public string? GoogleId { get; set; } 

        public UserRole Role { get; set; } = UserRole.User;
        public UserSettings UserSettings { get; private set; }

        public User(string email, string? passwordHash = null, string? googleId = null)
        {
            Email = email;
            PasswordHash = passwordHash;
            GoogleId = googleId;
            UserSettings = new UserSettings(false, true, true, true, Currency.PLN);
        }
    }
}
