namespace ExpenseSplitter.Application.DTOs.Groups.Response
{
    public record RemoveMemberResponse(string email, Guid groupId)
    {
    }
}
