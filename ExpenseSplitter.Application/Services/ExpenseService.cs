using ExpenseSplitter.Application.DTOs.Expense.Request;
using ExpenseSplitter.Application.DTOs.Expense.Response;
using ExpenseSplitter.Application.Interfaces;
using ExpenseSplitter.Application.Interfaces.Users;
using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Domain.Exceptions.Expense;

namespace ExpenseSplitter.Application.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IExpenseCategoryService _expenseCategoryService;

        public ExpenseService(ICurrentUserService currentUserService, IExpenseRepository expenseRepository, IExpenseCategoryService expenseCategoryService)
        {
            _currentUserService = currentUserService;
            _expenseRepository = expenseRepository;
            _expenseCategoryService = expenseCategoryService;
        }


        public async Task<CreateExpenseResponse> CreateAsync(CreateExpenseRequest request)
        {
            var expenseId = Guid.NewGuid();
            var userId = _currentUserService.UserId;

            if (!request.currency.HasValue ||
                !Enum.IsDefined(request.currency.Value))
            {
                throw new InvalidExpenseCurrencyException();
            }

            var currency = request.currency.Value;

            var expense = new Expense(
                expenseId,
                request.name,
                userId,
                request.categoryId,
                currency,
                request.amount,
                request.date);

            if (request.allocation is not null)
            {
                var allocation = new ExpenseAllocation(
                    expense.Id,
                    request.allocation.groupId,
                    request.allocation.participantUserIds);

                expense.AddAllocation(allocation);
            }

            await _expenseRepository.AddAsync(expense);
            await _expenseRepository.SaveChangesAsync();

            return new CreateExpenseResponse(expense.Id);
        }

        public async Task<UpdateExpenseRespone> UpdateAsync(UpdateExpenseRequest request)
        {
            var expense = await _expenseRepository.FindByIdAsync(request.id);

            if (expense is null)
            {
                throw new ExpenseNotFoundException();
            }

            if (expense.UserIdAdded != _currentUserService.UserId)
            {
                throw new UserIsNotExpenseOwnerException();
            }

            expense.Update(
                request.newName,
                request.amount,
                request.categoryId,
                request.newDate,
                request.currency);

            await _expenseRepository.UpdateAsync(expense);
            await _expenseRepository.SaveChangesAsync();

            return new UpdateExpenseRespone(expense.Id);
        }

        public async Task<RemoveExpenseRespone> RemoveAsync(RemoveExpenseRequest request)
        {
            var expense = await _expenseRepository.FindByIdAsync(request.id);

            if (expense is null)
            {
                throw new ExpenseNotFoundException();
            }

            if (expense.UserIdAdded != _currentUserService.UserId)
            {
                throw new UserIsNotExpenseOwnerException();
            }

            await _expenseRepository.DeleteAsync(expense);
            await _expenseRepository.SaveChangesAsync();

            return new RemoveExpenseRespone(expense.Id);
        }
    }
}