namespace ExpenseSplitter.Application.DTOs.Authentication
{
    public record RegisterRequest(
        string Email,
        string Password,
        string ConfirmPassword)
    {

    }
}
