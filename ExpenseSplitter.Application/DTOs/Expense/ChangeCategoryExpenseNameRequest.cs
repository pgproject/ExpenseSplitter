namespace ExpenseSplitter.Application.DTOs.Expense
{
    public record ChangeCategoryExpenseNameRequest(string currentName, string newName)
    {
    }
}
