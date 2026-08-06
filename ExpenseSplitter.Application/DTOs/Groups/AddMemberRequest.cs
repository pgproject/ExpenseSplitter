namespace ExpenseSplitter.Application.DTOs.Groups
{
    public record AddMemberRequest(string newMemberEmail, Guid existGroupId)
    {
    }
}
