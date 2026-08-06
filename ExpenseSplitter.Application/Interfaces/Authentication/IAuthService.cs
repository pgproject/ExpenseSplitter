using ExpenseSplitter.Application.DTOs.Authentication;

namespace ExpenseSplitter.Application.Interfaces.Authentication
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterUserAsync(RegisterRequest registerUserRequest);
        Task<LoginResponse> LoginUserAsync(LoginRequest registerUserRequest);
    }
}
