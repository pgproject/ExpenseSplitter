using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Domain.Exceptions.Expense;

namespace ExpenseSplitter.Domain.Tests.Entities.Expense
{
    public class ExpenseAllocationTests
    {
        [Fact]
        public void Constructor_Should_CreateAllocation_WhenDataIsValid()
        {
            var expenseId = Guid.NewGuid();
            var groupId = Guid.NewGuid();
            var participants = new List<Guid>
        {
            Guid.NewGuid(),
            Guid.NewGuid()
        };

            var allocation = new ExpenseAllocation(
                expenseId,
                groupId,
                participants);

            Assert.Equal(expenseId, allocation.ExpenseId);
            Assert.Equal(groupId, allocation.GroupId);
            Assert.Equal(participants, allocation.ParticipantUserIds);
        }

        [Fact]
        public void Constructor_Should_CopyParticipantList()
        {
            var participants = new List<Guid> { Guid.NewGuid() };

            var allocation = new ExpenseAllocation(
                Guid.NewGuid(),
                Guid.NewGuid(),
                participants);

            participants.Add(Guid.NewGuid());

            Assert.Single(allocation.ParticipantUserIds);
        }

        [Fact]
        public void Constructor_Should_Throw_WhenExpenseIdIsEmpty()
        {
            Assert.Throws<InvalidExpenseAllocationException>(() =>
                new ExpenseAllocation(
                    Guid.Empty,
                    Guid.NewGuid(),
                    new List<Guid> { Guid.NewGuid() }));
        }

        [Fact]
        public void Constructor_Should_Throw_WhenGroupIdIsEmpty()
        {
            Assert.Throws<InvalidExpenseAllocationException>(() =>
                new ExpenseAllocation(
                    Guid.NewGuid(),
                    Guid.Empty,
                    new List<Guid> { Guid.NewGuid() }));
        }

        [Fact]
        public void Constructor_Should_Throw_WhenParticipantsAreNull()
        {
            Assert.Throws<InvalidExpenseAllocationException>(() =>
                new ExpenseAllocation(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    null!));
        }

        [Fact]
        public void Constructor_Should_Throw_WhenParticipantsAreEmpty()
        {
            Assert.Throws<InvalidExpenseAllocationException>(() =>
                new ExpenseAllocation(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    new List<Guid>()));
        }

        [Fact]
        public void Constructor_Should_Throw_WhenParticipantIdIsEmpty()
        {
            Assert.Throws<InvalidExpenseAllocationException>(() =>
                new ExpenseAllocation(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    new List<Guid> { Guid.Empty }));
        }

        [Fact]
        public void Constructor_Should_Throw_WhenParticipantIdsAreDuplicated()
        {
            var participantId = Guid.NewGuid();

            Assert.Throws<InvalidExpenseAllocationException>(() =>
                new ExpenseAllocation(
                    Guid.NewGuid(),
                    Guid.NewGuid(),
                    new List<Guid> { participantId, participantId }));
        }
    }
}
