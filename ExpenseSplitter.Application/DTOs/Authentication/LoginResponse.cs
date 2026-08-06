using ExpenseSplitter.Domain.Enums;

namespace ExpenseSplitter.Application.DTOs.Authentication
{
    public record LoginResponse(
        Guid UserId,
        string Email,
        UserRole Role,
        string AccessToken)
    {
    }
}
