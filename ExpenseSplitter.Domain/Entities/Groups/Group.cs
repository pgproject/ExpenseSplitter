using ExpenseSplitter.Domain.Enums;
using ExpenseSplitter.Domain.Exceptions.Group;

namespace ExpenseSplitter.Domain.Entities.Groups
{
    public class Group
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; } = null!;

        private readonly List<GroupMember> _members = new();
        public IReadOnlyCollection<GroupMember> Members => _members;

        public GroupSettings Settings { get; private set; } = null!;

        private Group()
        {
        }

        public Group(string name, Guid ownerGruopId, GroupSettings settings)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidGroupNameException();
            }
            if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            Id = Guid.NewGuid();
            Name = name;
            Settings = settings;
            CreateGroupOwner(ownerGruopId);
        }

        public void AddMember(Guid currentUserId, Guid newMemberId)
        {
            GroupMember member = GetMemberByIdOrThrow(currentUserId);

            if (HasMember(newMemberId))
            {
                throw new GroupAlreadyContainsMemberException();
            }

            if (!Settings.OnlyOwnerCanAddMember)
            {
                CreateNewMember(newMemberId, GroupMemberRole.Member);
            }
            else if (IsOwner(member))
            {
                CreateNewMember(newMemberId, GroupMemberRole.Member);
            }
            else
            {
                throw new OnlyGroupOwnerCanAddMembersException();
            }
        }

        public void RemoveMember(Guid currentUserId, Guid memberToRemoveId)
        {
            GroupMember member = GetMemberByIdOrThrow(currentUserId);

            if (member.UserId == memberToRemoveId && IsOwner(member))
            {
                throw new OwnerCannotLeaveGroupException();
            }

            GroupMember memberToRemove = GetMemberByIdOrThrow(memberToRemoveId);

            if (IsOwner(memberToRemove))
            {
                throw new CannotRemoveGroupOwnerException();
            }

            if (!Settings.OnlyOwnerCanRemoveMember)
            {
                _members.Remove(memberToRemove);
            }
            else if (IsOwner(member))
            {
                _members.Remove(memberToRemove);
            }
            else
            {
                throw new OnlyGroupOwnerCanRemoveMembersException();
            }
        }

        public void ChangeOwnership(Guid currentOwnerId, Guid newOwnerId)
        {
            if (newOwnerId == currentOwnerId)
            {
                throw new CannotTransferOwnershipToCurrentOwnerException();
            }

            GroupMember currentOwner = GetMemberByIdOrThrow(currentOwnerId);

            if (!IsOwner(currentOwner))
            {
                throw new OnlyGroupOwnerCanTransferOwnershipException();
            }

            GroupMember newOwner = GetMemberByIdOrThrow(newOwnerId);

            currentOwner.SetRole(GroupMemberRole.Member);
            newOwner.SetRole(GroupMemberRole.Owner);
        }

        private bool IsOwner(GroupMember member)
        {
            return member.Role == GroupMemberRole.Owner;
        }

        private bool HasMember(Guid newUserId)
        {
            return _members.Any(x => x.UserId == newUserId);
        }

        private GroupMember GetMemberByIdOrThrow(Guid id)
        {
            return _members.FirstOrDefault(x => x.UserId == id)
                ?? throw new UserIsNotGroupMemberException();
        }

        private void CreateGroupOwner(Guid ownerUserId)
        {
            CreateNewMember(ownerUserId, GroupMemberRole.Owner);
        }

        private void CreateNewMember(Guid newMemberId, GroupMemberRole role)
        {
            GroupMember newMember = new GroupMember(Id, newMemberId, role);

            _members.Add(newMember);
        }

    }
}
