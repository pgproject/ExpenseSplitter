using ExpenseSplitter.Domain.Enums;

namespace ExpenseSplitter.Domain.Entities.Groups
{
    public class GroupMember
    {
        public Guid GroupId { get; private set; }
        public Guid UserId { get; private set; }
        public GroupMemberRole Role { get; private set; } = GroupMemberRole.Member;
        private GroupMember()
        {
        }
        public GroupMember(Guid groupId, Guid userId, GroupMemberRole role)
        {
            GroupId = groupId;
            UserId = userId;
            Role = role;
        }
        public void SetRole(GroupMemberRole role)
        {
            Role = role;
        }
    }
}
