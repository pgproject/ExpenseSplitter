using ExpenseSplitter.Application.DTOs.Groups.Request;
using ExpenseSplitter.Application.DTOs.Groups.Response;

namespace ExpenseSplitter.Application.Interfaces.Groups
{
    public interface IGroupService
    {
        Task<CreateGroupResponse> CreateAsync(CreateGroupRequest request);
        Task<AddMemberResponse> AddMember(AddMemberRequest request);
        Task<RemoveMemberResponse> RemoveMember(RemoveMemberRequest request);
        Task<ChangeOwnershipResponse> ChangeOwnership(ChangeOwnershipRequest request);
    }
}
