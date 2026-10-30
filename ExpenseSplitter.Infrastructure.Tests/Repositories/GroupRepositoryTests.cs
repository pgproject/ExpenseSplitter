using ExpenseSplitter.Domain.Entities.Groups;
using ExpenseSplitter.Domain.Enums;
using ExpenseSplitter.Infrastructure.Data;
using ExpenseSplitter.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ExpenseSplitter.Infrastructure.Tests.Repositories
{
    public class GroupRepositoryTests
    {

        [Fact]
        public async Task FindByIdAsync_Should_LoadGroupMembers()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql("YOUR_TEST_DATABASE_CONNECTION_STRING")
                .Options;

            var ownerId = Guid.NewGuid();
            var member1Id = Guid.NewGuid();
            var member2Id = Guid.NewGuid();

            var group = new Group(
                "Test group",
                ownerId,
                new GroupSettings(Currency.PLN));

            group.AddMember(ownerId, member1Id);
            group.AddMember(ownerId, member2Id);

            Guid groupId = group.Id;

            // Act
            await using (var context = new AppDbContext(options))
            {
                await context.Groups.AddAsync(group);
                await context.SaveChangesAsync();
            }

            await using (var context = new AppDbContext(options))
            {
                var repository = new GroupRepository(context);
                var result = await repository.FindByIdAsync(groupId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(3, result.Members.Count);

                Assert.Contains(result.Members, x => x.UserId == ownerId);
                Assert.Contains(result.Members, x => x.UserId == member1Id);
                Assert.Contains(result.Members, x => x.UserId == member2Id);
            }

            // Cleanup
            await using (var context = new AppDbContext(options))
            {
                var groupToRemove = await context.Groups
                    .FirstOrDefaultAsync(x => x.Id == groupId);

                if (groupToRemove != null)
                {
                    context.Groups.Remove(groupToRemove);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
