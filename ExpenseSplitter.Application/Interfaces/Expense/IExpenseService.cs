using ExpenseSplitter.Application.DTOs.Expense.Request;
using ExpenseSplitter.Application.DTOs.Expense.Response;
namespace ExpenseSplitter.Application.Interfaces
{
    public interface IExpenseService
    {
        Task<CreateExpenseResponse> CreateAsync(CreateExpenseRequest request);
        Task<UpdateExpenseRespone> UpdateAsync(UpdateExpenseRequest request);
        Task<RemoveExpenseRespone> RemoveAsync(RemoveExpenseRequest request);
    }
}
