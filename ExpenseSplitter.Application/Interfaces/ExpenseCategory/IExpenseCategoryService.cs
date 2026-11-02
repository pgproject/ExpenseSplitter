using ExpenseSplitter.Application.DTOs.ExpenseCategory.Request;
using ExpenseSplitter.Application.DTOs.ExpenseCategory.Response;

namespace ExpenseSplitter.Application.Interfaces
{
    public interface IExpenseCategoryService
    {
        Task<CreateExpenseCategoryResponse> CreateCategoryExpense(CreateCategoryExpenseRequest request);
        Task<UpdateExpenseCategoryResponse> UpdateCategoryExpense(UpdateExpenseCategoryRequest request);
        Task<RemoveExpenseCategoryResponse> RemoveCategoryExpense(RemoveCategoryExpenseRequest request);
        Task<List<GetExpenseCategoriesResponse>> GetAllAsync();
    }
}
