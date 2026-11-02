namespace ExpenseSplitter.Application.DTOs.Groups.Request
{
    public record AddMemberRequest(string newMemberEmail, Guid existGroupId)
    {
    }
}
