namespace ExpenseSplitter.Application.DTOs.Groups.Response
{
    public record GetGroupDetailsResponse(Guid groupId,
    string name,
    Guid ownerId,
    List<GroupMemberResponse> members,
    GroupSettingsResponse settings)
    {
    }
}
