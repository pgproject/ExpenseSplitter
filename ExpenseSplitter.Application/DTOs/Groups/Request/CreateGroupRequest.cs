using ExpenseSplitter.Domain.Entities.Groups;

namespace ExpenseSplitter.Application.DTOs.Groups.Request
{
    public record CreateGroupRequest(string name, CreateGroupSettingsRequest settings)
    {
    }
}
