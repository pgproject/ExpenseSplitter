using ExpenseSplitter.Application.DTOs.Expense;
using ExpenseSplitter.Application.Interfaces.Expense;
using ExpenseSplitter.Application.Interfaces.Users;
using ExpenseSplitter.Domain.Entities.Expense;
using ExpenseSplitter.Domain.Entities.User;
using ExpenseSplitter.Domain.Exceptions.Expense.Category;
using ExpenseSplitter.Domain.Exceptions.User;

namespace ExpenseSplitter.Application.Services
{
    public class ExpenseCategoryService : IExpenseCategoryService
    {
        private readonly IExpenseCategoryRepository _expenseCategoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRepository _userRepository;
        public ExpenseCategoryService(IExpenseCategoryRepository expenseCategoryRepository, ICurrentUserService currentUserService, IUserRepository userRepository)
        {
            _expenseCategoryRepository = expenseCategoryRepository;
            _currentUserService = currentUserService;
            _userRepository = userRepository;
        }

        public async Task<CreateCategoryExpenseRequest> CreateCategoryExpense(CreateCategoryExpenseRequest request)
        {
            var currentUserId = _currentUserService.UserId;
            User user = await GetUserOrThrowExceptionAsync(currentUserId);
            if (!user.UserSettings.CanCreateExpanseCategory)
            {
                throw new ThisUserIsNotAllowedToCreateExpenseCategoryException();
            }

            ExpenseCategory? findCategoryWithName = await _expenseCategoryRepository.FindByNameAsync(request.name, currentUserId);
            if (findCategoryWithName != null)
            {
                throw new ExpenseCategoryNameAlreadyExistsException();
            }

            ExpenseCategory? findPublicCategoryWithNewName = await _expenseCategoryRepository.FindPublicByNameAsync(request.name);
            if (findPublicCategoryWithNewName != null)
            {
                throw new ExpenseCategoryNameAlreadyExistsException();
            }

            ExpenseCategory newCategory = new ExpenseCategory(request.name, currentUserId);

            await _expenseCategoryRepository.AddAsync(newCategory);
            await _expenseCategoryRepository.SaveChangesAsync();
            return new CreateCategoryExpenseRequest(newCategory.Name);
        }

        public async Task<ChangeCategoryExpenseNameRequest> ChangeCategoryExpenseName(ChangeCategoryExpenseNameRequest request)
        {
            var currentUserId = _currentUserService.UserId;
            User user = await GetUserOrThrowExceptionAsync(currentUserId);
            if (!user.UserSettings.CanEditExpenseCategory)
            {
                throw new ThisUserIsNotAllowedToEditExpenseCategoryException();
            }

            ExpenseCategory? expenseCategory = await _expenseCategoryRepository.FindByNameAsync(request.currentName, currentUserId);
            if (expenseCategory == null) 
            {
                throw new InvalidCategoryExpenseException();
            }
            if (expenseCategory.Name == request.newName)
            {
                throw new ExpenseCategoryNameUnchangedException();
            }

            ExpenseCategory? findCategoryWithNewName = await _expenseCategoryRepository.FindByNameAsync(request.newName, currentUserId);
            if (findCategoryWithNewName != null)
            {
                throw new ExpenseCategoryNameAlreadyExistsException();
            }

            ExpenseCategory? findPublicCategoryWithNewName = await _expenseCategoryRepository.FindPublicByNameAsync(request.newName);
            if (findPublicCategoryWithNewName != null)
            {
                throw new ExpenseCategoryNameAlreadyExistsException();
            }
            var oldName = expenseCategory.Name;


            expenseCategory.ChangeName(request.newName);
            await _expenseCategoryRepository.SaveChangesAsync();

            return new ChangeCategoryExpenseNameRequest(oldName, request.newName);
        }

        public async Task<RemoveCategoryExpenseRequest> RemoveCategoryExpense(RemoveCategoryExpenseRequest request)
        {
            var currentUserId = _currentUserService.UserId;
            User user = await GetUserOrThrowExceptionAsync(currentUserId);
            if (!user.UserSettings.CanRemoveExpenseCategory)
            {
                throw new ThisUserIsNotAllowedToDeleteExpenseCategoryException();
            }
            ExpenseCategory? expenseCategory = await _expenseCategoryRepository.FindByNameAsync(request.categoryName, currentUserId);

            if (expenseCategory == null)
            {
                throw new InvalidCategoryExpenseException();
            }

            await _expenseCategoryRepository.DeleteAsync(expenseCategory);
            await _expenseCategoryRepository.SaveChangesAsync();

            return new RemoveCategoryExpenseRequest(expenseCategory.Name);
        }

        private async Task<User> GetUserOrThrowExceptionAsync(Guid userId)
        {
            User? user = await _userRepository.FindByIdAsync(userId);

            if (user == null)
            {
                throw new UserNotFoundException(userId.ToString());
            }

            return user;
        }
    }
}
