using ExpenseSplitter.Domain.Entities.Groups;

namespace ExpenseSplitter.Application.DTOs.Groups
{
    public record CreateGroupRequest(string name, GroupSettings settings)
    {
    }
}
