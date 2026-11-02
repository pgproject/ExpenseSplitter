namespace ExpenseSplitter.Application.DTOs.ExpenseCategory.Response
{
    public record GetExpenseCategoriesResponse(Guid categoryId,
        string categoryName,
        bool isPublic)
    {
    }
}
