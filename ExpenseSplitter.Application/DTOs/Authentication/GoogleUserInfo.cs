namespace ExpenseSplitter.Application.DTOs.Authentication
{
    public record GoogleUserInfo(string googleId, string email, bool emailVerified)
    {
    }
}
