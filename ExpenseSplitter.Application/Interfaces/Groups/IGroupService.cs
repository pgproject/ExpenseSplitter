using ExpenseSplitter.Application.DTOs.Groups;

namespace ExpenseSplitter.Application.Interfaces.Groups
{
    public interface IGroupService
    {
        Task<CreateGroupRequest> CreateAsync(CreateGroupRequest request);
        Task<AddMemberRequest> AddMember(AddMemberRequest request);
        Task<RemoveMemberRequest> RemoveMember(RemoveMemberRequest request);
        Task<ChangeOwnershipRequest> ChangeOwnership(ChangeOwnershipRequest request);
    }
}
