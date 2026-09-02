using ExpenseSplitter.Application.DTOs.Groups;
using ExpenseSplitter.Application.Interfaces.Groups;
using ExpenseSplitter.Application.Interfaces.Users;
using ExpenseSplitter.Application.Services;
using ExpenseSplitter.Domain.Entities.Groups;
using ExpenseSplitter.Domain.Entities.User;
using ExpenseSplitter.Domain.Enums;
using ExpenseSplitter.Domain.Exceptions.Group;
using ExpenseSplitter.Domain.Exceptions.User;
using Moq;

namespace ExpenseSplitter.Application.Test
{
    public class GroupServiceTest
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
        private readonly Mock<IGroupRepository> _groupRepositoryMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();

        private readonly GroupService _groupService;

        public GroupServiceTest()
        {
            _groupService = new GroupService(
                _currentUserServiceMock.Object,
                _groupRepositoryMock.Object, 
                _userRepositoryMock.Object
                );
        }

        [Fact]
        public async Task CreateAsync_Should_CreateGroup_When_RequestIsValid()
        {
            var ownerId = Guid.NewGuid();

            _currentUserServiceMock.Setup(x => x.UserId).Returns(ownerId);

            var request = new CreateGroupRequest("Trip to Italy", new GroupSettings(Currency.PLN));
            
            var result = await _groupService.CreateAsync(request);

            Assert.Equal(request.name, result.name);

            _groupRepositoryMock.Verify(x => x.AddAsync(It.Is<Group>(group => 
                group.Name == request.name && 
                group.Members.Single().UserId == ownerId &&
                group.Members.Single().Role == GroupMemberRole.Owner)),
                Times.Once);

            _groupRepositoryMock.Verify(x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_NameIsEmpty()
        {
            var request = new CreateGroupRequest("", new GroupSettings(Currency.PLN));

            await Assert.ThrowsAsync<InvalidGroupNameException>(() =>
                 _groupService.CreateAsync(request));

            _groupRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Group>()),
                Times.Never);

            _groupRepositoryMock.Verify(x => x.SaveChangesAsync(),
                Times.Never);
        }


        [Fact]
        public async Task CreateAsync_Should_Throw_When_SettingsIsNull()
        {
            var request = new CreateGroupRequest("Italy", null!);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                 _groupService.CreateAsync(request));

            _groupRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Group>()),
                Times.Never);

            _groupRepositoryMock.Verify(x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task AddMember_Should_AddMember_When_RequestIsValid()
        {
            var ownerId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var group = new Group(
                "Trip to Italy",
                ownerId,
                new GroupSettings(Currency.PLN));

            var user = new User(
                "hashs",
                "password");

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(ownerId);

            _groupRepositoryMock
                .Setup(x => x.FindByIdAsync(groupId))
                .ReturnsAsync(group);

            _userRepositoryMock
                .Setup(x => x.FindByEmailAsync("hashs"))
                .ReturnsAsync(user);

            var request = new AddMemberRequest("hashs", groupId);

            var result = await _groupService.AddMember(request);

            Assert.Equal("hashs", result.newMemberEmail);
            Assert.Equal(group.Id, result.existGroupId);

            Assert.Contains(group.Members, member =>
                member.UserId == user.Id &&
                member.Role == GroupMemberRole.Member);

            _groupRepositoryMock.Verify(
                x => x.FindByIdAsync(groupId),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.FindByEmailAsync("hashs"),
                Times.Once);

            _groupRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task AddMember_Should_Throw_When_GroupDoesNotExist()
        {
            var ownerId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(ownerId);

            _groupRepositoryMock
                .Setup(x => x.FindByIdAsync(groupId))
                .ReturnsAsync((Group?)null);

            var request = new AddMemberRequest("hashs", groupId);

            await Assert.ThrowsAsync<GroupNotFoundException>(() =>
                _groupService.AddMember(request));

            _groupRepositoryMock.Verify(
                x => x.FindByIdAsync(groupId),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.FindByEmailAsync(It.IsAny<string>()),
                Times.Never);

            _groupRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task AddMember_Should_Throw_When_UserDoesNotExist()
        {
            var ownerId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var group = new Group(
                "Trip to Italy",
                ownerId,
                new GroupSettings(Currency.PLN));

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(ownerId);

            _groupRepositoryMock
                .Setup(x => x.FindByIdAsync(groupId))
                .ReturnsAsync(group);

            _userRepositoryMock
                .Setup(x => x.FindByEmailAsync("hashs"))
                .ReturnsAsync((User?)null);

            var request = new AddMemberRequest("hashs", groupId);

            await Assert.ThrowsAsync<UserNotFoundException>(() =>
                _groupService.AddMember(request));

            _groupRepositoryMock.Verify(
                x => x.FindByIdAsync(groupId),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.FindByEmailAsync("hashs"),
                Times.Once);

            _groupRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task AddMember_Should_Throw_When_CurrentUserIsNotGroupMember()
        {
            var ownerId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var group = new Group(
                "Trip to Italy",
                ownerId,
                new GroupSettings(Currency.PLN));

            var user = new User(
                "hashs",
                "password");

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(currentUserId);

            _groupRepositoryMock
                .Setup(x => x.FindByIdAsync(groupId))
                .ReturnsAsync(group);

            _userRepositoryMock
                .Setup(x => x.FindByEmailAsync("hashs"))
                .ReturnsAsync(user);

            var request = new AddMemberRequest("hashs", groupId);

            await Assert.ThrowsAsync<UserIsNotGroupMemberException>(() =>
                _groupService.AddMember(request));

            _groupRepositoryMock.Verify(
                x => x.FindByIdAsync(groupId),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.FindByEmailAsync("hashs"),
                Times.Once);

            _groupRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task AddMember_Should_Throw_When_CurrentUserIsNotOwner()
        {
            var ownerId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var group = new Group(
                "Trip to Italy",
                ownerId,
                new GroupSettings(Currency.PLN));

            group.AddMember(ownerId, currentUserId);

            group.Settings.ChangeSettings(true, false, Currency.PLN);

            var user = new User(
                "hashs",
                "password");

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(currentUserId);

            _groupRepositoryMock
                .Setup(x => x.FindByIdAsync(groupId))
                .ReturnsAsync(group);

            _userRepositoryMock
                .Setup(x => x.FindByEmailAsync("hashs"))
                .ReturnsAsync(user);

            var request = new AddMemberRequest("hashs", groupId);

            await Assert.ThrowsAsync<OnlyGroupOwnerCanAddMembersException>(() =>
                _groupService.AddMember(request));

            _groupRepositoryMock.Verify(
                x => x.FindByIdAsync(groupId),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.FindByEmailAsync("hashs"),
                Times.Once);

            _groupRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task AddMember_Should_Throw_When_UserIsAlreadyMember()
        {
            var ownerId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var group = new Group(
                "Trip to Italy",
                ownerId,
                new GroupSettings(Currency.PLN));


            var user = new User(
                "hashs",
                "password");

            group.AddMember(ownerId, user.Id);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(ownerId);

            _groupRepositoryMock
                .Setup(x => x.FindByIdAsync(groupId))
                .ReturnsAsync(group);

            _userRepositoryMock
                .Setup(x => x.FindByEmailAsync("hashs"))
                .ReturnsAsync(user);

            var request = new AddMemberRequest("hashs", groupId);

            await Assert.ThrowsAsync<GroupAlreadyContainsMemberException>(() =>
                _groupService.AddMember(request));

            _groupRepositoryMock.Verify(
                x => x.FindByIdAsync(groupId),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.FindByEmailAsync("hashs"),
                Times.Once);

            _groupRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task ChangeOwnership_Should_ChangeOwner_When_RequestIsValid()
        {
            var currentOwnerId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var group = new Group(
                "Trip to Italy",
                currentOwnerId,
                new GroupSettings(Currency.PLN));

            var user = new User(
                "newowner@test.com",
                "password");

            group.AddMember(currentOwnerId, user.Id);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(currentOwnerId);

            _groupRepositoryMock
                .Setup(x => x.FindByIdAsync(groupId))
                .ReturnsAsync(group);

            _userRepositoryMock
                .Setup(x => x.FindByEmailAsync("newowner@test.com"))
                .ReturnsAsync(user);

            var request = new ChangeOwnershipRequest(
                "newowner@test.com",
                groupId);

            var result = await _groupService.ChangeOwnership(request);

            Assert.Equal("newowner@test.com", result.newOwnerOfGroupEmail);
            Assert.Equal(group.Id, result.existGroupId);

            Assert.Equal(
                GroupMemberRole.Member,
                group.Members.Single(x => x.UserId == currentOwnerId).Role);

            Assert.Equal(
                GroupMemberRole.Owner,
                group.Members.Single(x => x.UserId == user.Id).Role);

            _groupRepositoryMock.Verify(
                x => x.FindByIdAsync(groupId),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.FindByEmailAsync("newowner@test.com"),
                Times.Once);

            _groupRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task ChangeOwnership_Should_Throw_When_GroupDoesNotExist()
        {
            var groupId = Guid.NewGuid();

            _groupRepositoryMock
                .Setup(x => x.FindByIdAsync(groupId))
                .ReturnsAsync((Group?)null);

            var request = new ChangeOwnershipRequest(
                "newowner@test.com",
                groupId);

            await Assert.ThrowsAsync<GroupNotFoundException>(() =>
                _groupService.ChangeOwnership(request));

            _groupRepositoryMock.Verify(
                x => x.FindByIdAsync(groupId),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.FindByEmailAsync(It.IsAny<string>()),
                Times.Never);

            _groupRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task ChangeOwnership_Should_Throw_When_NewOwnerDoesNotExist()
        {
            var currentOwnerId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var group = new Group(
                "Trip to Italy",
                currentOwnerId,
                new GroupSettings(Currency.PLN));

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(currentOwnerId);

            _groupRepositoryMock
                .Setup(x => x.FindByIdAsync(groupId))
                .ReturnsAsync(group);

            _userRepositoryMock
                .Setup(x => x.FindByEmailAsync("newowner@test.com"))
                .ReturnsAsync((User?)null);

            var request = new ChangeOwnershipRequest(
                "newowner@test.com",
                groupId);

            await Assert.ThrowsAsync<UserNotFoundException>(() =>
                _groupService.ChangeOwnership(request));

            _groupRepositoryMock.Verify(
                x => x.FindByIdAsync(groupId),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.FindByEmailAsync("newowner@test.com"),
                Times.Once);

            _groupRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task ChangeOwnership_Should_Throw_When_CurrentUserIsNotOwner()
        {
            var ownerId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var group = new Group(
                "Trip to Italy",
                ownerId,
                new GroupSettings(Currency.PLN));

            group.AddMember(ownerId, memberId);

            var newOwner = new User(
                "newowner@test.com",
                "password");

            group.AddMember(ownerId, newOwner.Id);
            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(memberId);

            _groupRepositoryMock
                .Setup(x => x.FindByIdAsync(groupId))
                .ReturnsAsync(group);

            _userRepositoryMock
                .Setup(x => x.FindByEmailAsync("newowner@test.com"))
                .ReturnsAsync(newOwner);

            var request = new ChangeOwnershipRequest(
                "newowner@test.com",
                groupId);

            await Assert.ThrowsAsync<OnlyGroupOwnerCanTransferOwnershipException>(() =>
                _groupService.ChangeOwnership(request));

            _groupRepositoryMock.Verify(
                x => x.FindByIdAsync(groupId),
                Times.Once);

            _groupRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);

            _userRepositoryMock.Verify(
                x => x.FindByEmailAsync("newowner@test.com"),
                Times.Once);
        }

        [Fact]
        public async Task ChangeOwnership_Should_Throw_When_CurrentOwnerTransfersOwnershipToHimself()
        {
            var ownerId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var group = new Group(
                "Trip to Italy",
                ownerId,
                new GroupSettings(Currency.PLN));

            var user = new User(
                "owner@test.com",
                "password");

            user.Id = ownerId;

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(ownerId);

            _groupRepositoryMock
                .Setup(x => x.FindByIdAsync(groupId))
                .ReturnsAsync(group);

            _userRepositoryMock
                .Setup(x => x.FindByEmailAsync("owner@test.com"))
                .ReturnsAsync(user);

            var request = new ChangeOwnershipRequest(
                "owner@test.com",
                groupId);

            await Assert.ThrowsAsync<CannotTransferOwnershipToCurrentOwnerException>(() =>
                _groupService.ChangeOwnership(request));

            _groupRepositoryMock.Verify(
                x => x.FindByIdAsync(groupId),
                Times.Once);

            _userRepositoryMock.Verify(
                x => x.FindByEmailAsync("owner@test.com"),
                Times.Once);

            _groupRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task ChangeOwnership_Should_Throw_When_NewOwnerIsNotGroupMember()
        {
            var ownerId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            var group = new Group(
                "Trip to Italy",
                ownerId,
                new GroupSettings(Currency.PLN));

            var user = new User(
                "newowner@test.com",
                "password");

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(ownerId);

            _groupRepositoryMock
                .Setup(x => x.FindByIdAsync(groupId))
                .ReturnsAsync(group);

            _userRepositoryMock
                .Setup(x => x.FindByEmailAsync("newowner@test.com"))
                .ReturnsAsync(user);

            var request = new ChangeOwnershipRequest(
                "newowner@test.com",
                groupId);

            await Assert.ThrowsAsync<UserIsNotGroupMemberException>(() =>
                _groupService.ChangeOwnership(request));


            _userRepositoryMock.Verify(
                x => x.FindByEmailAsync("newowner@test.com"),
                Times.Once);

            _groupRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);

            _groupRepositoryMock.Verify(
                x => x.FindByIdAsync(groupId),
                Times.Once);
        }
    }
}
