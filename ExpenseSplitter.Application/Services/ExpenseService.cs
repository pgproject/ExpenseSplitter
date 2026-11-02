using ExpenseSplitter.Application.DTOs.Expense.Request;
using ExpenseSplitter.Application.DTOs.Expense.Response;
using ExpenseSplitter.Application.Interfaces;
using ExpenseSplitter.Application.Interfaces.Groups;
using ExpenseSplitter.Application.Interfaces.Users;
using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Domain.Exceptions.Expense;
using ExpenseSplitter.Domain.Exceptions.Group;

namespace ExpenseSplitter.Application.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IExpenseCategoryService _expenseCategoryService;
        private readonly IGroupRepository _groupRepository;


        public ExpenseService(ICurrentUserService currentUserService, IExpenseRepository expenseRepository, IExpenseCategoryService expenseCategoryService, IGroupRepository groupRepository)
        {
            _currentUserService = currentUserService;
            _expenseRepository = expenseRepository;
            _expenseCategoryService = expenseCategoryService;
            _groupRepository = groupRepository;
        }


        public async Task<CreateExpenseResponse> CreateAsync(CreateExpenseRequest request)
        {
            var expenseId = Guid.NewGuid();
            var userId = _currentUserService.UserId;

            await _expenseCategoryService.ValidateCategoryAccessAsync(request.categoryId);

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

        public async Task<UpdateExpenseResponse> UpdateAsync(UpdateExpenseRequest request)
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

            await _expenseCategoryService.ValidateCategoryAccessAsync(request.categoryId);

            expense.Update(
                request.newName,
                request.amount,
                request.categoryId,
                request.newDate,
                request.currency);

            await _expenseRepository.UpdateAsync(expense);
            await _expenseRepository.SaveChangesAsync();

            return new UpdateExpenseResponse(expense.Id);
        }

        public async Task<RemoveExpenseResponse> RemoveAsync(RemoveExpenseRequest request)
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

            return new RemoveExpenseResponse(expense.Id);
        }

        public async Task<List<GetExpensesResponse>> GetAllAsync()
        {
            var userId = _currentUserService.UserId;

            var expenses = await _expenseRepository.FindAllForUserAsync(userId);

            return expenses.Select(expense => new GetExpensesResponse(
                expense.Id,
                expense.Name,
                expense.Amount,
                expense.CategoryId,
                expense.UserIdAdded,
                expense.Date,
                (int)expense.Currency,
                expense.ExpenseAllocations.Select(allocation =>
                    new ExpenseAllocationResponse(
                        allocation.GroupId,
                        allocation.ParticipantUserIds
                    )
                ).ToList()
            )).ToList();
        }

        public async Task<List<GetExpensesResponse>> GetByGroupAsync(Guid groupId)
        {
            var userId = _currentUserService.UserId;

            var group = await _groupRepository.FindByIdAsync(groupId);

            if (group is null)
            {
                throw new GroupNotFoundException(groupId);
            }

            if (!group.Members.Any(member => member.UserId == userId))
            {
                throw new UserIsNotGroupMemberException();
            }

            var expenses = await _expenseRepository.FindAllForGroup(groupId);

            return expenses.Select(MapToResponse).ToList();
        }

        private static GetExpensesResponse MapToResponse(Expense expense)
        {
            return new GetExpensesResponse(
                expense.Id,
                expense.Name,
                expense.Amount,
                expense.CategoryId,
                expense.UserIdAdded,
                expense.Date,
                (int)expense.Currency,
                expense.ExpenseAllocations.Select(allocation =>
                    new ExpenseAllocationResponse(
                        allocation.GroupId,
                        allocation.ParticipantUserIds
                    )
                ).ToList()
            );
        }
    }
}