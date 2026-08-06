namespace ExpenseSplitter.Application.DTOs.Groups
{
    public record RemoveMemberRequest(string emailMemberToRemove, Guid existGroupId)
    {
    }
}
