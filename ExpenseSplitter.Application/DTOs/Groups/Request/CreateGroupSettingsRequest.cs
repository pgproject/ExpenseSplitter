namespace ExpenseSplitter.Application.DTOs.Groups.Request
{
    public record CreateGroupSettingsRequest(bool onlyOwnerCanAddMember,
        bool onlyOwnerCanRemoveMember)
    {
    }
}
