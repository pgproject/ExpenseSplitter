using ExpenseSplitter.Domain.Enums;

namespace ExpenseSplitter.Application.DTOs.Groups.Response
{
    public record GroupMemberResponse(Guid userId,
    string email,
    string role)
    {
    }
}
