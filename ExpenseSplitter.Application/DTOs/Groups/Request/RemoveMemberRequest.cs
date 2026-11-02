namespace ExpenseSplitter.Application.DTOs.Groups.Request
{
    public record RemoveMemberRequest(string emailMemberToRemove, Guid existGroupId)
    {
    }
}
