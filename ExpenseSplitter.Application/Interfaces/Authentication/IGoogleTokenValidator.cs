using ExpenseSplitter.Application.DTOs.Authentication;

namespace ExpenseSplitter.Application.Interfaces.Authentication
{
    public interface IGoogleTokenValidator
    {
        Task<GoogleUserInfo> ValidateAsync(string idToken);
    }
}
