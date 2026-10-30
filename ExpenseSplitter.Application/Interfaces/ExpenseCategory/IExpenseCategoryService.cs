using ExpenseSplitter.Application.DTOs.ExpenseCategory;

namespace ExpenseSplitter.Application.Interfaces
{
    public interface IExpenseCategoryService
    {
        Task<CreateCategoryExpenseRequest> CreateCategoryExpense(CreateCategoryExpenseRequest request);
        Task<ChangeCategoryExpenseNameRequest> ChangeCategoryExpenseName(ChangeCategoryExpenseNameRequest request);
        Task<RemoveCategoryExpenseRequest> RemoveCategoryExpense(RemoveCategoryExpenseRequest request);
    }
}
