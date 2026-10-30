namespace ExpenseSplitter.Domain.Exceptions.Expense
{
    public class UserIsNotExpenseOwnerException : Exception
    {
        public UserIsNotExpenseOwnerException() : base("Only the user who created the expense can remove it.")
        {
        }
    }
}
