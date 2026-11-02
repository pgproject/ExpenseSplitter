using ExpenseSplitter.Application.DTOs.Expense.Request;
using ExpenseSplitter.Application.DTOs.Expense.Response;
namespace ExpenseSplitter.Application.Interfaces
{
    public interface IExpenseService
    {
        Task<CreateExpenseResponse> CreateAsync(CreateExpenseRequest request);
        Task<UpdateExpenseResponse> UpdateAsync(UpdateExpenseRequest request);
        Task<RemoveExpenseResponse> RemoveAsync(RemoveExpenseRequest request);
        Task<List<GetExpensesResponse>> GetAllAsync();

        Task<List<GetExpensesResponse>> GetByGroupAsync(Guid groupId);
    }
}
