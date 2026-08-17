using ExpenseSplitter.Application.DTOs.Expense;

namespace ExpenseSplitter.Application.Interfaces.Expense
{
    public interface IExpenseCategoryService
    {
        Task<CreateCategoryExpenseRequest> CreateCategoryExpense(CreateCategoryExpenseRequest request);
        Task<ChangeCategoryExpenseNameRequest> ChangeCategoryExpenseName(ChangeCategoryExpenseNameRequest request);
        Task<RemoveCategoryExpenseRequest> RemoveCategoryExpense(RemoveCategoryExpenseRequest request);
    }
}
