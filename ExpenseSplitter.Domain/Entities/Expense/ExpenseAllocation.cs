using ExpenseSplitter.Domain.Exceptions.Expense;

namespace ExpenseSplitter.Domain.Entities
{
    public class ExpenseAllocation
    {
        public Guid ExpenseId { get; private set; }

        public Guid GroupId { get; private set; }

        public List<Guid> ParticipantUserIds { get; private set; } = new();
        private ExpenseAllocation()
        {
        }

        public ExpenseAllocation(Guid expenseId, Guid groupId, List<Guid> participantUserIds)
        {
            if (expenseId == Guid.Empty ||
                groupId == Guid.Empty ||
                participantUserIds == null ||
                participantUserIds.Count == 0 ||
                participantUserIds.Any(id => id == Guid.Empty) ||
                participantUserIds.Distinct().Count() != participantUserIds.Count)
            {
                throw new InvalidExpenseAllocationException();
            }

            ExpenseId = expenseId;
            GroupId = groupId;
            ParticipantUserIds = new List<Guid>(participantUserIds);
        }
    }
}
