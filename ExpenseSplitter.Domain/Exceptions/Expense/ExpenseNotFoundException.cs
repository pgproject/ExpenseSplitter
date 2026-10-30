namespace ExpenseSplitter.Domain.Exceptions.Expense
{
    public class ExpenseNotFoundException : Exception
    {
        public ExpenseNotFoundException() : base("Expense was not found.")
        {
        }
    }
}
