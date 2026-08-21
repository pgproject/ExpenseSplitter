using ExpenseSplitter.Application.DTOs.Expense;
using ExpenseSplitter.Application.Interfaces.Expense;
using ExpenseSplitter.Application.Interfaces.Users;
using ExpenseSplitter.Application.Services;
using ExpenseSplitter.Domain.Entities.Expense;
using ExpenseSplitter.Domain.Entities.User;
using ExpenseSplitter.Domain.Exceptions.Expense.Category;
using ExpenseSplitter.Domain.Exceptions.User;
using Moq;

namespace ExpenseSplitter.Application.Test
{
    public class ExpenseCategoryServiceTest
    {

        private readonly Mock<IExpenseCategoryRepository> _expenseCategoryRepositoryMock = new();
        private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();

        private readonly ExpenseCategoryService _expenseCategoryService;

        public ExpenseCategoryServiceTest()
        {
            _expenseCategoryService = new ExpenseCategoryService(
                _expenseCategoryRepositoryMock.Object,
                _currentUserServiceMock.Object,
                _userRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateCategoryExpense_Should_CreateCategory_When_RequestIsValid()
        {
            var userId = Guid.NewGuid();

            var user = new User(
                "user@test.com",
                "password");

            user.Id = userId;

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var request = new CreateCategoryExpenseRequest("Car");

            var result = await _expenseCategoryService.CreateCategoryExpense(request);

            Assert.Equal("Car", result.name);

            _expenseCategoryRepositoryMock.Verify(
                x => x.FindByNameAsync("Car", userId),
                Times.Once);

            _expenseCategoryRepositoryMock.Verify(
                x => x.FindPublicByNameAsync("Car"),
                Times.Once);

            _expenseCategoryRepositoryMock.Verify(
                x => x.AddAsync(It.Is<ExpenseCategory>(category =>
                    category.Name == "Car" &&
                    category.UserId == userId)),
                Times.Once);

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateCategoryExpense_Should_Throw_When_UserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync((User?)null);

            var request = new CreateCategoryExpenseRequest("Car");

            await Assert.ThrowsAsync<UserNotFoundException>(() =>
                _expenseCategoryService.CreateCategoryExpense(request));

            _expenseCategoryRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<ExpenseCategory>()),
                Times.Never);

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task CreateCategoryExpense_Should_Throw_When_UserIsNotAllowedToCreateCategory()
        {
            var userId = Guid.NewGuid();

            var user = new User(
                "user@test.com",
                "password");

            user.Id = userId;

            user.UserSettings.ChangeUserSettings(
                new UserSettings(false, false, true, true));

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var request = new CreateCategoryExpenseRequest("Car");

            await Assert.ThrowsAsync<ThisUserIsNotAllowedToCreateExpenseCategoryException>(() =>
                _expenseCategoryService.CreateCategoryExpense(request));

            _expenseCategoryRepositoryMock.Verify(
                x => x.FindByNameAsync(It.IsAny<string>(), It.IsAny<Guid>()),
                Times.Never);

            _expenseCategoryRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<ExpenseCategory>()),
                Times.Never);

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task CreateCategoryExpense_Should_Throw_When_PrivateCategoryWithSameNameExists()
        {
            var userId = Guid.NewGuid();

            var user = new User(
                "user@test.com",
                "password");

            user.Id = userId;

            var existingCategory = new ExpenseCategory(
                "Car",
                userId);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _expenseCategoryRepositoryMock
                .Setup(x => x.FindByNameAsync("Car", userId))
                .ReturnsAsync(existingCategory);

            var request = new CreateCategoryExpenseRequest("Car");

            await Assert.ThrowsAsync<ExpenseCategoryNameAlreadyExistsException>(() =>
                _expenseCategoryService.CreateCategoryExpense(request));

            _expenseCategoryRepositoryMock.Verify(
                x => x.FindPublicByNameAsync(It.IsAny<string>()),
                Times.Never);

            _expenseCategoryRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<ExpenseCategory>()),
                Times.Never);

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task CreateCategoryExpense_Should_Throw_When_PublicCategoryWithSameNameExists()
        {
            var userId = Guid.NewGuid();

            var user = new User(
                "user@test.com",
                "password");

            user.Id = userId;

            var publicCategory = new ExpenseCategory(
                "Car",
                null);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _expenseCategoryRepositoryMock
                .Setup(x => x.FindPublicByNameAsync("Car"))
                .ReturnsAsync(publicCategory);

            var request = new CreateCategoryExpenseRequest("Car");

            await Assert.ThrowsAsync<ExpenseCategoryNameAlreadyExistsException>(() =>
                _expenseCategoryService.CreateCategoryExpense(request));

            _expenseCategoryRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<ExpenseCategory>()),
                Times.Never);

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task ChangeCategoryExpenseName_Should_ChangeName_When_RequestIsValid()
        {
            var userId = Guid.NewGuid();

            var user = new User(
                "user@test.com",
                "password");

            user.Id = userId;

            var category = new ExpenseCategory(
                "Car",
                userId);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _expenseCategoryRepositoryMock
                .Setup(x => x.FindByNameAsync("Car", userId))
                .ReturnsAsync(category);

            _expenseCategoryRepositoryMock
                .Setup(x => x.FindByNameAsync("Sport", userId))
                .ReturnsAsync((ExpenseCategory?)null);

            _expenseCategoryRepositoryMock
                .Setup(x => x.FindPublicByNameAsync("Sport"))
                .ReturnsAsync((ExpenseCategory?)null);

            var request = new ChangeCategoryExpenseNameRequest(
                "Car",
                "Sport");

            var result = await _expenseCategoryService.ChangeCategoryExpenseName(request);

            Assert.Equal("Sport", result.newName);
            Assert.Equal("Sport", category.Name);
            Assert.Equal("Car", result.currentName);

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task ChangeCategoryExpenseName_Should_Throw_When_UserIsNotAllowedToEditCategory()
        {
            var userId = Guid.NewGuid();

            var user = new User(
                "user@test.com",
                "password");

            user.Id = userId;

            user.UserSettings.ChangeUserSettings(
                new UserSettings(false, true, false, true));

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var request = new ChangeCategoryExpenseNameRequest(
                "Car",
                "Sport");

            await Assert.ThrowsAsync<ThisUserIsNotAllowedToEditExpenseCategoryException>(() =>
                _expenseCategoryService.ChangeCategoryExpenseName(request));

            _expenseCategoryRepositoryMock.Verify(
                x => x.FindByNameAsync(It.IsAny<string>(), It.IsAny<Guid>()),
                Times.Never);

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task ChangeCategoryExpenseName_Should_Throw_When_CategoryDoesNotExist()
        {
            var userId = Guid.NewGuid();

            var user = new User(
                "user@test.com",
                "password");

            user.Id = userId;

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _expenseCategoryRepositoryMock
                .Setup(x => x.FindByNameAsync("Car", userId))
                .ReturnsAsync((ExpenseCategory?)null);

            var request = new ChangeCategoryExpenseNameRequest(
                "Car",
                "Sport");

            await Assert.ThrowsAsync<InvalidCategoryExpenseException>(() =>
                _expenseCategoryService.ChangeCategoryExpenseName(request));

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task ChangeCategoryExpenseName_Should_Throw_When_NewNameIsSameAsCurrentName()
        {
            var userId = Guid.NewGuid();

            var user = new User(
                "user@test.com",
                "password");

            user.Id = userId;

            var category = new ExpenseCategory(
                "Car",
                userId);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _expenseCategoryRepositoryMock
                .Setup(x => x.FindByNameAsync("Car", userId))
                .ReturnsAsync(category);

            var request = new ChangeCategoryExpenseNameRequest(
                "Car",
                "Car");

            await Assert.ThrowsAsync<ExpenseCategoryNameUnchangedException>(() =>
                _expenseCategoryService.ChangeCategoryExpenseName(request));

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task ChangeCategoryExpenseName_Should_Throw_When_NewNameAlreadyExistsForUser()
        {
            var userId = Guid.NewGuid();

            var user = new User(
                "user@test.com",
                "password");

            user.Id = userId;

            var currentCategory = new ExpenseCategory(
                "Car",
                userId);

            var existingCategory = new ExpenseCategory(
                "Sport",
                userId);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _expenseCategoryRepositoryMock
                .Setup(x => x.FindByNameAsync("Car", userId))
                .ReturnsAsync(currentCategory);

            _expenseCategoryRepositoryMock
                .Setup(x => x.FindByNameAsync("Sport", userId))
                .ReturnsAsync(existingCategory);

            var request = new ChangeCategoryExpenseNameRequest(
                "Car",
                "Sport");

            await Assert.ThrowsAsync<ExpenseCategoryNameAlreadyExistsException>(() =>
                _expenseCategoryService.ChangeCategoryExpenseName(request));

            _expenseCategoryRepositoryMock.Verify(
                x => x.FindPublicByNameAsync(It.IsAny<string>()),
                Times.Never);

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task ChangeCategoryExpenseName_Should_Throw_When_NewNameAlreadyExistsAsPublicCategory()
        {
            var userId = Guid.NewGuid();

            var user = new User(
                "user@test.com",
                "password");

            user.Id = userId;

            var currentCategory = new ExpenseCategory(
                "Car",
                userId);

            var publicCategory = new ExpenseCategory(
                "Sport",
                null);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _expenseCategoryRepositoryMock
                .Setup(x => x.FindByNameAsync("Car", userId))
                .ReturnsAsync(currentCategory);

            _expenseCategoryRepositoryMock
                .Setup(x => x.FindByNameAsync("Sport", userId))
                .ReturnsAsync((ExpenseCategory?)null);

            _expenseCategoryRepositoryMock
                .Setup(x => x.FindPublicByNameAsync("Sport"))
                .ReturnsAsync(publicCategory);

            var request = new ChangeCategoryExpenseNameRequest(
                "Car",
                "Sport");

            await Assert.ThrowsAsync<ExpenseCategoryNameAlreadyExistsException>(() =>
                _expenseCategoryService.ChangeCategoryExpenseName(request));

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task RemoveCategoryExpense_Should_RemoveCategory_When_RequestIsValid()
        {
            var userId = Guid.NewGuid();

            var user = new User(
                "user@test.com",
                "password");

            user.Id = userId;

            var category = new ExpenseCategory(
                "Car",
                userId);

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _expenseCategoryRepositoryMock
                .Setup(x => x.FindByNameAsync("Car", userId))
                .ReturnsAsync(category);

            var request = new RemoveCategoryExpenseRequest("Car");

            var result = await _expenseCategoryService.RemoveCategoryExpense(request);

            Assert.Equal("Car", result.categoryName);

            _expenseCategoryRepositoryMock.Verify(
                x => x.DeleteAsync(category),
                Times.Once);

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task RemoveCategoryExpense_Should_Throw_When_UserIsNotAllowedToRemoveCategory()
        {
            var userId = Guid.NewGuid();

            var user = new User(
                "user@test.com",
                "password");

            user.Id = userId;

            user.UserSettings.ChangeUserSettings(
                new UserSettings(false, true, true, false));

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var request = new RemoveCategoryExpenseRequest("Car");

            await Assert.ThrowsAsync<ThisUserIsNotAllowedToDeleteExpenseCategoryException>(() =>
                _expenseCategoryService.RemoveCategoryExpense(request));

            _expenseCategoryRepositoryMock.Verify(
                x => x.FindByNameAsync(It.IsAny<string>(), It.IsAny<Guid>()),
                Times.Never);

            _expenseCategoryRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<ExpenseCategory>()),
                Times.Never);

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task RemoveCategoryExpense_Should_Throw_When_CategoryDoesNotExist()
        {
            var userId = Guid.NewGuid();

            var user = new User(
                "user@test.com",
                "password");

            user.Id = userId;

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _expenseCategoryRepositoryMock
                .Setup(x => x.FindByNameAsync("Car", userId))
                .ReturnsAsync((ExpenseCategory?)null);

            var request = new RemoveCategoryExpenseRequest("Car");

            await Assert.ThrowsAsync<InvalidCategoryExpenseException>(() =>
                _expenseCategoryService.RemoveCategoryExpense(request));

            _expenseCategoryRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<ExpenseCategory>()),
                Times.Never);

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task ChangeCategoryExpenseName_Should_Throw_When_UserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync((User?)null);

            var request = new ChangeCategoryExpenseNameRequest(
                "Car",
                "Sport");

            await Assert.ThrowsAsync<UserNotFoundException>(() =>
                _expenseCategoryService.ChangeCategoryExpenseName(request));

            _expenseCategoryRepositoryMock.Verify(
                x => x.FindByNameAsync(It.IsAny<string>(), It.IsAny<Guid>()),
                Times.Never);

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task RemoveCategoryExpense_Should_Throw_When_UserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            _currentUserServiceMock
                .Setup(x => x.UserId)
                .Returns(userId);

            _userRepositoryMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync((User?)null);

            var request = new RemoveCategoryExpenseRequest("Car");

            await Assert.ThrowsAsync<UserNotFoundException>(() =>
                _expenseCategoryService.RemoveCategoryExpense(request));

            _expenseCategoryRepositoryMock.Verify(
                x => x.FindByNameAsync(It.IsAny<string>(), It.IsAny<Guid>()),
                Times.Never);

            _expenseCategoryRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<ExpenseCategory>()),
                Times.Never);

            _expenseCategoryRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}
