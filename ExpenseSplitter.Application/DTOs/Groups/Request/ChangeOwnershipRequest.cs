namespace ExpenseSplitter.Application.DTOs.Groups.Request
{
    public record ChangeOwnershipRequest(string newOwnerOfGroupEmail, Guid existGroupId)
    {
    }
}
