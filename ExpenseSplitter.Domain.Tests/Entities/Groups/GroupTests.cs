using ExpenseSplitter.Domain.Entities.Groups;
using ExpenseSplitter.Domain.Enums;
using ExpenseSplitter.Domain.Exceptions.Group;

namespace ExpenseSplitter.Domain.Tests.Entities.Groups
{
    public class GroupTests
    {
        [Fact]
        public void Constructor_Should_CreateGroupWithOwner()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            var owner = group.Members.Single();

            Assert.Equal("Trip to Italy", group.Name);
            Assert.Single(group.Members);
            Assert.Equal(ownerId, owner.UserId);
            Assert.Equal(GroupMemberRole.Owner, owner.Role);
        }

        [Fact]
        public void Constructor_Should_Throw_When_NameIsEmpty()
        {
            var ownerId = Guid.NewGuid();

            var settings = new GroupSettings();

            Assert.Throws<InvalidGroupNameException>(() =>
            {
                new Group("", ownerId, settings);
            });
        }

        [Fact]
        public void Constructor_Should_Throw_When_SettingsIsNull()
        {
            var ownerId = Guid.NewGuid();

            Assert.Throws<ArgumentNullException>(() =>
            {
                new Group("Italy", ownerId, null!);
            });
        }

        [Fact]
        public void AddMember_Should_AddNewMember_When_DataIsValid()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid newUserId = Guid.NewGuid();
            group.AddMember(ownerId, newUserId);

            Assert.Collection(group.Members, 
                owner =>
                {
                    Assert.Equal(ownerId, owner.UserId);
                    Assert.Equal(GroupMemberRole.Owner, owner.Role);
                },
                member =>
                {
                    Assert.Equal(newUserId, member.UserId);
                    Assert.Equal(GroupMemberRole.Member, member.Role);
                });
        }

        [Fact]
        public void AddMember_Should_Throw_When_UserIsAlreadyMember()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid newUserId = Guid.NewGuid();
            group.AddMember(ownerId, newUserId);

            Assert.Throws<GroupAlreadyContainsMemberException>(() =>
            {
                group.AddMember(ownerId, newUserId);
            });
        }

        [Fact]
        public void AddMember_Should_Throw_When_OnlyOwnerCanAddMembers()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid newUserId = Guid.NewGuid();
            group.AddMember(ownerId, newUserId);

            Guid secondUserId = Guid.NewGuid();

            group.Settings.ChangeSettings(true, false);

            Assert.Throws<OnlyGroupOwnerCanAddMembersException>(() =>
            {
                group.AddMember(newUserId, secondUserId);
            });
        }

        [Fact]
        public void AddMember_Should_AddMember_When_AnyMemberCanAdd()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid newUserId = Guid.NewGuid();
            group.AddMember(ownerId, newUserId);

            Guid secondUserId = Guid.NewGuid();
            group.AddMember(newUserId, secondUserId);

            Assert.Collection(group.Members,
                owner =>
                {
                    Assert.Equal(ownerId, owner.UserId);
                    Assert.Equal(GroupMemberRole.Owner, owner.Role);
                },
                member =>
                {
                    Assert.Equal(newUserId, member.UserId);
                    Assert.Equal(GroupMemberRole.Member, member.Role);
                },
                member =>
                {
                    Assert.Equal(secondUserId, member.UserId);
                    Assert.Equal(GroupMemberRole.Member, member.Role);
                }
                );

        }

        private Group CreateDefaultGroup(Guid ownerId) 
            => new Group("Trip to Italy", ownerId, new GroupSettings());
    }
}