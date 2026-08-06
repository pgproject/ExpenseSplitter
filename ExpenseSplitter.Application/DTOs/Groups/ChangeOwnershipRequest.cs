namespace ExpenseSplitter.Application.DTOs.Groups
{
    public record ChangeOwnershipRequest(string newOwnerOfGroupEmail, Guid existGroupId)
    {
    }
}
