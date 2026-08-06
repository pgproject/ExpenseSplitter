namespace ExpenseSplitter.Domain.Exceptions.Authentication
{
    public class EmailAlreadyExistsException : Exception
    {
        public EmailAlreadyExistsException() :base("User with this email already exists.")
        {

        }
    }
}
