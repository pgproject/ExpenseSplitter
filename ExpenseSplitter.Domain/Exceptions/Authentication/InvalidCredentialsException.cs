namespace ExpenseSplitter.Domain.Exceptions.Authentication
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException() : base("Invalid credentials.") 
        {
        }
    }
}
