using ExpenseSplitter.Application.DTOs.Groups;
using ExpenseSplitter.Application.Interfaces.Groups;
using ExpenseSplitter.Application.Interfaces.Users;
using ExpenseSplitter.Application.Services;
using ExpenseSplitter.Domain.Entities.Groups;
using ExpenseSplitter.Domain.Enums;
using ExpenseSplitter.Domain.Exceptions.Group;
using Moq;

namespace ExpenseSplitter.Application.Test
{
    public class GroupServiceTest
    {
        private readonly Mock<ICurrentUserService> _cureentUserServiceMock = new();
        private readonly Mock<IGroupRepository> _groupRepositoryMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();

        private readonly GroupService _groupService;

        private GroupServiceTest()
        {
            _groupService = new GroupService(
                _cureentUserServiceMock.Object,
                _groupRepositoryMock.Object, 
                _userRepositoryMock.Object
                );
        }

        [Fact]
        public async Task CreateAsync_Should_CreateGroup_When_RequestIsValid()
        {
            var ownerId = Guid.NewGuid();

            _cureentUserServiceMock.Setup(x => x.UserId).Returns(ownerId);

            var request = new CreateGroupRequest("Trip to Italy", new GroupSettings());
            
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
            var request = new CreateGroupRequest("", new GroupSettings());

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
    }
}
