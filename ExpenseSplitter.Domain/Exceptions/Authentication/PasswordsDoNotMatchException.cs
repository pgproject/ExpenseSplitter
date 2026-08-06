namespace ExpenseSplitter.Domain.Exceptions.Authentication
{
    public class PasswordsDoNotMatchException : Exception
    {
        public PasswordsDoNotMatchException() : base("Password are not the same") 
        {
        }
    }
}
