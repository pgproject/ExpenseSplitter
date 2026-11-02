namespace ExpenseSplitter.Application.DTOs.Groups.Response
{
    public record ChangeOwnershipResponse(string newOwnerEmail, Guid groupId)
    {
    }
}
