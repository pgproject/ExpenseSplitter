namespace ExpenseSplitter.Application.DTOs.ExpenseCategory.Response
{
    public record UpdateExpenseCategoryResponse(Guid categoryId, string oldName, string newName)
    {
    }
}
