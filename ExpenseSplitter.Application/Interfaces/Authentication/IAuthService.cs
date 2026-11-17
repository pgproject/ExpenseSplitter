using ExpenseSplitter.Application.DTOs.Authentication;

namespace ExpenseSplitter.Application.Interfaces.Authentication
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterUserAsync(RegisterRequest registerUserRequest);
        Task<LoginResponse> LoginUserAsync(LoginRequest registerUserRequest);
        Task<LoginResponse> LoginWithGoogleAsync(GoogleLoginRequest request);
        Task LinkGoogleAccountAsync(Guid userId, GoogleLoginRequest request);
        Task<bool> HasGoogleAccountAsync(Guid userId);
    }
}
