namespace ExpenseSplitter.Domain.Exceptions.Group
{
    public class GroupNotFoundException : Exception
    {
        public GroupNotFoundException(Guid id) : base("There is no group with id " + id)
        {
        }
    }
}
