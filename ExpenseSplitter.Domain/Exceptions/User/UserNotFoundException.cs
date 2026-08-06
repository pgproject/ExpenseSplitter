namespace ExpenseSplitter.Domain.Exceptions.User
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string reason) : base(reason)
        {
        }
    }
}
