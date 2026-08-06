namespace ExpenseSplitter.Application.DTOs.Authentication
{
    public record LoginRequest(
        string Email,
        string Password
        )
    {
    }
}
