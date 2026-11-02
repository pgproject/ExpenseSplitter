namespace ExpenseSplitter.Application.DTOs.ExpenseCategory.Request
{
    public record UpdateExpenseCategoryRequest(Guid categoryId, string newName)
    {
    }
}
