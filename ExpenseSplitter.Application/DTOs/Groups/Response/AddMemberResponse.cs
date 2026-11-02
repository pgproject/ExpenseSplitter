namespace ExpenseSplitter.Application.DTOs.Groups.Response
{
    public record AddMemberResponse(string email, Guid groupId)
    {
    }
}
