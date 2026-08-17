using ExpenseSplitter.Domain.Enums;

namespace ExpenseSplitter.Domain.Entities.User
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public UserRole Role { get; set; } = UserRole.User;
        public UserSettings UserSettings { get; private set; }

        public User(string email, string passwordHash)
        {
            Email = email;
            PasswordHash = passwordHash;
            UserSettings = new UserSettings(false, true, true);
        }
    }
}
