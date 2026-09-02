using ExpenseSplitter.Domain.Entities.Groups;
using ExpenseSplitter.Domain.Enums;
using ExpenseSplitter.Domain.Exceptions.Group;

namespace ExpenseSplitter.Domain.Tests.Entities.Groups
{
    public class GroupTests
    {
        #region Constructors 
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

        #endregion

        #region AddMember

        [Fact]
        public void AddMember_Should_AddMember_When_OwnerAddsMember()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid firstMemberId = Guid.NewGuid();
            group.AddMember(ownerId, firstMemberId);

            Assert.Collection(group.Members, 
                owner =>
                {
                    Assert.Equal(ownerId, owner.UserId);
                    Assert.Equal(GroupMemberRole.Owner, owner.Role);
                },
                member =>
                {
                    Assert.Equal(firstMemberId, member.UserId);
                    Assert.Equal(GroupMemberRole.Member, member.Role);
                });
        }

        [Fact]
        public void AddMember_Should_Throw_When_UserIsAlreadyMember()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid firstMemberId = Guid.NewGuid();
            group.AddMember(ownerId, firstMemberId);

            Assert.Throws<GroupAlreadyContainsMemberException>(() =>
            {
                group.AddMember(ownerId, firstMemberId);
            });
        }

        [Fact]
        public void AddMember_Should_Throw_When_OnlyOwnerCanAddMembers()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid firstMemberId = Guid.NewGuid();
            group.AddMember(ownerId, firstMemberId);

            Guid secondUserId = Guid.NewGuid();

            group.Settings.ChangeSettings(true, false, Currency.PLN);

            Assert.Throws<OnlyGroupOwnerCanAddMembersException>(() =>
            {
                group.AddMember(firstMemberId, secondUserId);
            });
        }

        [Fact]
        public void AddMember_Should_AddMember_When_MemberAddsMember()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid firstMemberId = Guid.NewGuid();
            group.AddMember(ownerId, firstMemberId);

            Guid secondUserId = Guid.NewGuid();
            group.AddMember(firstMemberId, secondUserId);

            Assert.Collection(group.Members,
                owner =>
                {
                    Assert.Equal(ownerId, owner.UserId);
                    Assert.Equal(GroupMemberRole.Owner, owner.Role);
                },
                member =>
                {
                    Assert.Equal(firstMemberId, member.UserId);
                    Assert.Equal(GroupMemberRole.Member, member.Role);
                },
                member =>
                {
                    Assert.Equal(secondUserId, member.UserId);
                    Assert.Equal(GroupMemberRole.Member, member.Role);
                });
        }

        #endregion

        #region RemoveMember
        [Fact] 
        public void RemoveMember_Should_RemoveMember_When_OwnerRemovesMember()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid firstMemberId = Guid.NewGuid();
            group.AddMember(ownerId, firstMemberId);

            group.RemoveMember(ownerId, firstMemberId);

            Assert.Single(group.Members);
            Assert.Collection(group.Members,
                owner =>
                {
                    Assert.Equal(ownerId, owner.UserId);
                    Assert.Equal(GroupMemberRole.Owner, owner.Role);
                });
        }

        [Fact]
        public void RemoveMember_Should_RemoveMember_When_OnlyOwnerCanRemove()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid firstMemberId = Guid.NewGuid();
            group.AddMember(ownerId, firstMemberId);

            group.Settings.ChangeSettings(false, true, Currency.PLN);
            group.RemoveMember(ownerId, firstMemberId);

            Assert.Single(group.Members);
            Assert.Collection(group.Members,
                owner =>
                {
                    Assert.Equal(ownerId, owner.UserId);
                    Assert.Equal(GroupMemberRole.Owner, owner.Role);
                });
        }

        [Fact]
        public void RemoveMember_Should_RemoveMember_When_MemberRemovesMember()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid firstMemberId = Guid.NewGuid();
            group.AddMember(ownerId, firstMemberId);

            Guid secondMemberId = Guid.NewGuid();
            group.AddMember(ownerId, secondMemberId);

            group.Settings.ChangeSettings(true, false, Currency.PLN);

            group.RemoveMember(firstMemberId, secondMemberId);

            Assert.Collection(group.Members,
               owner =>
               {
                   Assert.Equal(ownerId, owner.UserId);
                   Assert.Equal(GroupMemberRole.Owner, owner.Role);
               },
               member =>
               {
                   Assert.Equal(firstMemberId, member.UserId);
                   Assert.Equal(GroupMemberRole.Member, member.Role);
               });
        }

        [Fact]
        public void RemoveMember_Should_Throw_When_Owner_Remove_Owner()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Assert.Throws<OwnerCannotLeaveGroupException>(() =>
            {
                group.RemoveMember(ownerId, ownerId);

            });
        }

        [Fact]
        public void RemoveMember_Should_Throw_When_MemberRemovesOwner()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid firstMemberId = Guid.NewGuid();
            group.AddMember(ownerId, firstMemberId);

            Assert.Throws<CannotRemoveGroupOwnerException>(() =>
            {
                group.RemoveMember(firstMemberId, ownerId);
            });
        }

        [Fact]
        public void RemoveMember_Should_Throw_When_MemberRemovesMember()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid firstMemberId = Guid.NewGuid();
            group.AddMember(ownerId, firstMemberId);

            Guid secondMemberId = Guid.NewGuid();
            group.AddMember(ownerId, secondMemberId);

            group.Settings.ChangeSettings(true, true, Currency.PLN);

            Assert.Throws<OnlyGroupOwnerCanRemoveMembersException>(() =>
            {
                group.RemoveMember(firstMemberId, secondMemberId);
            });
        }

        [Fact]
        public void RemoveMember_Should_Throw_When_NonMemberRemovesMember()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid firstMemberId = Guid.NewGuid();
            group.AddMember(ownerId, firstMemberId);

            Guid secondMemberId = Guid.NewGuid();

            Assert.Throws<UserIsNotGroupMemberException>(() =>
            {
                group.RemoveMember(secondMemberId, firstMemberId);
            });
        }

        [Fact]
        public void RemoveMember_Should_Throw_When_RemovingUserWhoIsNotGroupMember()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid firstMemberId = Guid.NewGuid();

            Assert.Throws<UserIsNotGroupMemberException>(() =>
            {
                group.RemoveMember(ownerId, firstMemberId);
            });
        }
        #endregion

        #region ChangeOwnership
        [Fact] 
        public void ChangeOwnership_Should_TransferOwnership_When_DataIsValid()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid firstMemberId = Guid.NewGuid();
            group.AddMember(ownerId, firstMemberId);

            group.ChangeOwnership(ownerId, firstMemberId);

            Assert.Collection(group.Members,
               owner =>
               {
                   Assert.Equal(ownerId, owner.UserId);
                   Assert.Equal(GroupMemberRole.Member, owner.Role);
               },
               member =>
               {
                   Assert.Equal(firstMemberId, member.UserId);
                   Assert.Equal(GroupMemberRole.Owner, member.Role);
            });
            Assert.Single(group.Members.Where(x => x.Role == GroupMemberRole.Owner));
        }

        [Fact] 
        public void ChangeOwnership_Should_Throw_When_Ownership_TransferToOwner()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Assert.Throws<CannotTransferOwnershipToCurrentOwnerException>(() =>
            {
                group.ChangeOwnership(ownerId, ownerId);
            });
        }

        [Fact]
        public void ChangeOwnership_Should_Throw_When_Member_TransferOwnership()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid firstMemberId = Guid.NewGuid();
            group.AddMember(ownerId, firstMemberId);

            Guid secondMemberId = Guid.NewGuid();
            group.AddMember(ownerId, secondMemberId);

            Assert.Throws<OnlyGroupOwnerCanTransferOwnershipException>(() =>
            {
                group.ChangeOwnership(firstMemberId, secondMemberId);
            });
        }

        [Fact]
        public void ChangeOwnership_Should_Throw_When_NonMemberTransfersOwnership()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid firstMemberId = Guid.NewGuid();
            group.AddMember(ownerId, firstMemberId);

            Guid secondMemberId = Guid.NewGuid();

            Assert.Throws<UserIsNotGroupMemberException>(() =>
            {
                group.ChangeOwnership(secondMemberId, firstMemberId);
            });
        }


        [Fact]
        public void ChangeOwnership_Should_Throw_When_NewOwnerIsNotGroupMember()
        {
            var ownerId = Guid.NewGuid();

            Group group = CreateDefaultGroup(ownerId);

            Guid firstMemberId = Guid.NewGuid();

            Assert.Throws<UserIsNotGroupMemberException>(() =>
            {
                group.ChangeOwnership(ownerId, firstMemberId);
            });
        }
        #endregion

        #region Helpers
        private Group CreateDefaultGroup(Guid ownerId) 
            => new Group("Trip to Italy", ownerId, new GroupSettings());

        #endregion
    }
}