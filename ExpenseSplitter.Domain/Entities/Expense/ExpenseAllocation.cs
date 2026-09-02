namespace ExpenseSplitter.Domain.Entities
{
    public class ExpenseAllocation
    {
        public Guid ExpenseId { get; private set; }

        public Guid GroupId { get; private set; }

        public List<Guid> ParticipantUserIds { get; private set; } = new();
    }
}
