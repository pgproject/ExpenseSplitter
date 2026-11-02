using ExpenseSplitter.Application.DTOs.Groups.Request;
using ExpenseSplitter.Application.DTOs.Groups.Response;
using ExpenseSplitter.Application.Interfaces.Groups;
using ExpenseSplitter.Application.Interfaces.Users;
using ExpenseSplitter.Domain.Entities.Groups;
using ExpenseSplitter.Domain.Entities.User;
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

        public async Task<CreateGroupResponse> CreateAsync(CreateGroupRequest request)
        {
            var currentUserId = _currentUserService.UserId;
            User user = await GetUserOrThrowExceptionAsync(currentUserId);

            GroupSettings settings = new GroupSettings(
         user.UserSettings.DefaultUserCurrency);

            settings.ChangeSettings(
                request.settings.onlyOwnerCanAddMember,
                request.settings.onlyOwnerCanRemoveMember,
                user.UserSettings.DefaultUserCurrency);

            Group newGroup = new Group(
                request.name,
                currentUserId,
                settings);

            await _groupRepository.AddAsync(newGroup);
            await _groupRepository.SaveChangesAsync();

            return new CreateGroupResponse(newGroup.Id, newGroup.Name);
        }

        public async Task<AddMemberResponse> AddMember(AddMemberRequest request)
        {
            Group group = await GetGroupOrThrowExceptionAsync(request.existGroupId);

            User user = await GetUserOrThrowExceptionAsync(request.newMemberEmail);

            group.AddMember(_currentUserService.UserId, user.Id);

            await _groupRepository.SaveChangesAsync();

            return new AddMemberResponse(user.Email, group.Id);
        }

        public async Task<RemoveMemberResponse> RemoveMember(RemoveMemberRequest request)
        {
            Group group = await GetGroupOrThrowExceptionAsync(request.existGroupId);

            User user = await GetUserOrThrowExceptionAsync(request.emailMemberToRemove);

            group.RemoveMember(_currentUserService.UserId, user.Id);

            await _groupRepository.SaveChangesAsync();

            return new RemoveMemberResponse(user.Email, group.Id);
        }

        public async Task<ChangeOwnershipResponse> ChangeOwnership(ChangeOwnershipRequest request)
        {
            Group group = await GetGroupOrThrowExceptionAsync(request.existGroupId);

            User user = await GetUserOrThrowExceptionAsync(request.newOwnerOfGroupEmail);

            group.ChangeOwnership(_currentUserService.UserId, user.Id);
            
            await _groupRepository.SaveChangesAsync();

            return new ChangeOwnershipResponse(user.Email, group.Id);
        }

        public async Task<List<GetGroupsResponse>> GetAllAsync()
        {
            var currentUserId = _currentUserService.UserId;

            List<Group> groups = await _groupRepository.FindAllForUserAsync(
                currentUserId);

            return groups.Select(group =>
            {
                GroupMember currentMember = group.Members
                    .First(member => member.UserId == currentUserId);

                return new GetGroupsResponse(
                    group.Id,
                    group.Name,
                    currentMember.Role.ToString(),
                    group.Members.Count
                );
            }).ToList();
        }

        public async Task<GetGroupDetailsResponse> GetByIdAsync(Guid groupId)
        {
            var currentUserId = _currentUserService.UserId;

            Group group = await GetGroupOrThrowExceptionAsync(groupId);

            if (!group.Members.Any(member => member.UserId == currentUserId))
            {
                throw new UserIsNotGroupMemberException();
            }

            var members = new List<GroupMemberResponse>();

            foreach (var member in group.Members)
            {
                User user = await GetUserOrThrowExceptionAsync(member.UserId);

                members.Add(new GroupMemberResponse(
                    user.Id,
                    user.Email,
                    member.Role.ToString()
                ));
            }

            var owner = group.Members.First(member =>
                member.Role == ExpenseSplitter.Domain.Enums.GroupMemberRole.Owner);

            return new GetGroupDetailsResponse(
                group.Id,
                group.Name,
                owner.UserId,
                members,
                new GroupSettingsResponse(
                    group.Settings.OnlyOwnerCanAddMember,
                    group.Settings.OnlyOwnerCanRemoveMember,
                    group.Settings.DefaultCurrency
                )
            );
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

        private async Task<User> GetUserOrThrowExceptionAsync(Guid userId)
        {
            User? user = await _userRepository.FindByIdAsync(userId);

            if (user == null)
            {
                throw new UserNotFoundException(userId.ToString());
            }

            return user;
        }
    }
}
