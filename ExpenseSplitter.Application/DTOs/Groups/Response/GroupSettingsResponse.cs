using ExpenseSplitter.Domain.Enums;

namespace ExpenseSplitter.Application.DTOs.Groups.Response
{
    public record GroupSettingsResponse(bool onlyOwnerCanAddMember,
    bool onlyOwnerCanRemoveMember,
    Currency defaultCurrency)
    {
    }
}
