using ExpenseSplitter.Application.DTOs.Groups;
using ExpenseSplitter.Application.Interfaces.Groups;
using ExpenseSplitter.Application.Interfaces.Users;
using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Domain.Entities.Groups;
using ExpenseSplitter.Domain.Exceptions.Group;
using ExpenseSplitter.Domain.Exceptions.User;

namespace ExpenseSplitter.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;

        public GroupService(ICurrentUserService currentUserService, IGroupRepository groupRepository, IUserRepository userRepository)
        {
            _currentUserService = currentUserService;
            _groupRepository = groupRepository;
            _userRepository = userRepository;
        }

        public async Task<CreateGroupRequest> CreateAsync(CreateGroupRequest request)
        {
            var currentUserId = _currentUserService.UserId;

            Group newGroup = new Group(request.name, _currentUserService.UserId, request.settings);

            await _groupRepository.AddAsync(newGroup);
            await _groupRepository.SaveChangesAsync();
            return new CreateGroupRequest(newGroup.Name, newGroup.Settings);
        }

        public async Task<AddMemberRequest> AddMember(AddMemberRequest request)
        {
            Group group = await GetGroupOrThrowExceptionAsync(request.existGroupId);

            User user = await GetUserOrThrowExceptionAsync(request.newMemberEmail);

            group.AddMember(_currentUserService.UserId, user.Id);

            await _groupRepository.SaveChangesAsync();

            return new AddMemberRequest(user.Email, group.Id);
        }

        public async Task<RemoveMemberRequest> RemoveMember(RemoveMemberRequest request)
        {
            Group group = await GetGroupOrThrowExceptionAsync(request.existGroupId);

            User user = await GetUserOrThrowExceptionAsync(request.emailMemberToRemove);

            group.RemoveMember(_currentUserService.UserId, user.Id);

            await _groupRepository.SaveChangesAsync();

            return new RemoveMemberRequest(user.Email, group.Id);
        }

        public async Task<ChangeOwnershipRequest> ChangeOwnership(ChangeOwnershipRequest request)
        {
            Group group = await GetGroupOrThrowExceptionAsync(request.existGroupId);

            User user = await GetUserOrThrowExceptionAsync(request.newOwnerOfGroupEmail);

            group.ChangeOwnership(_currentUserService.UserId, user.Id);
            
            await _groupRepository.SaveChangesAsync();

            return new ChangeOwnershipRequest(user.Email, group.Id);
        }

        private async Task<Group> GetGroupOrThrowExceptionAsync(Guid gruopId)
        {
            Group? group = await _groupRepository.FindByIdAsync(gruopId);

            if (group == null)
            {
                throw new GroupNotFoundException(gruopId);
            }

            return group;
        }

        private async Task<User> GetUserOrThrowExceptionAsync(string userEmail)
        {
            User? user = await _userRepository.FindByEmailAsync(userEmail);

            if (user == null)
            {
                throw new UserNotFoundException(userEmail);
            }

            return user;
        }
    }
}
