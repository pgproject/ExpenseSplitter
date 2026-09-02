namespace ExpenseSplitter.Application.DTOs.ExpenseCategory
{
    public record ChangeCategoryExpenseNameRequest(string currentName, string newName)
    {
    }
}
