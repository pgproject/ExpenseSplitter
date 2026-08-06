namespace ExpenseSplitter.Domain.Exceptions.Group
{
    public class GroupAlreadyContainsMemberException : Exception 
    {
        public GroupAlreadyContainsMemberException() : base($"User with this email belongs to this group.")
        { }
    }
}
