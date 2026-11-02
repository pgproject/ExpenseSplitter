namespace ExpenseSplitter.Application.DTOs.Groups.Response
{
    public record GetGroupsResponse(
        Guid groupId,
        string name,
        string currentUserRole,
        int membersCount)
    {
    }
}
