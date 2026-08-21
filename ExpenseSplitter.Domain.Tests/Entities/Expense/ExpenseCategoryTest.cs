using ExpenseSplitter.Domain.Entities.Expense;
using ExpenseSplitter.Domain.Exceptions.Expense.Category;

namespace ExpenseSplitter.Domain.Tests.Entities.Expense
{
    public class ExpenseCategoryTest
    {

        [Fact]
        public void Constructor_Should_CreatePrivateCategory_When_NameAndUserIdAreValid()
        {
            var userId = Guid.NewGuid();

            var category = new ExpenseCategory("Car", userId);

            Assert.NotEqual(Guid.Empty, category.Id);
            Assert.Equal("Car", category.Name);
            Assert.Equal(userId, category.UserId);
            Assert.False(category.IsPublic);
        }

        [Fact]
        public void Constructor_Should_CreatePublicCategory_When_UserIdIsNull()
        {
            var category = new ExpenseCategory("Car", null);

            Assert.NotEqual(Guid.Empty, category.Id);
            Assert.Equal("Car", category.Name);
            Assert.Null(category.UserId);
            Assert.True(category.IsPublic);
        }

        [Fact]
        public void Constructor_Should_Throw_When_NameIsEmpty()
        {
            Assert.Throws<InvalidExpenseCategoryNameException>(() =>
                new ExpenseCategory("", Guid.NewGuid()));
        }

        [Fact]
        public void Constructor_Should_Throw_When_NameIsNull()
        {
            Assert.Throws<InvalidExpenseCategoryNameException>(() =>
                new ExpenseCategory(null!, Guid.NewGuid()));
        }

        [Fact]
        public void Constructor_Should_Throw_When_NameContainsOnlyWhitespace()
        {
            Assert.Throws<InvalidExpenseCategoryNameException>(() =>
                new ExpenseCategory("   ", Guid.NewGuid()));
        }

        [Fact]
        public void ChangeName_Should_ChangeName_When_CategoryIsPrivate()
        {
            var category = new ExpenseCategory(
                "Car",
                Guid.NewGuid());

            category.ChangeName("Sport");

            Assert.Equal("Sport", category.Name);
        }

        [Fact]
        public void ChangeName_Should_Throw_When_NewNameIsEmpty()
        {
            var category = new ExpenseCategory(
                "Car",
                Guid.NewGuid());

            Assert.Throws<InvalidExpenseCategoryNameException>(() =>
                category.ChangeName(""));
        }

        [Fact]
        public void ChangeName_Should_Throw_When_NewNameContainsOnlyWhitespace()
        {
            var category = new ExpenseCategory(
                "Car",
                Guid.NewGuid());

            Assert.Throws<InvalidExpenseCategoryNameException>(() =>
                category.ChangeName("   "));
        }

        [Fact]
        public void ChangeName_Should_Throw_When_CategoryIsPublic()
        {
            var category = new ExpenseCategory(
                "Car",
                null);

            Assert.Throws<OnlyPrivateExpenseCategoryCanBeChangedException>(() =>
                category.ChangeName("Sport"));
        }

       

        [Fact]
        public void ChangeName_Should_Throw_When_NewNameIsNull()
        {
            var category = new ExpenseCategory(
                "Car",
                Guid.NewGuid());

            Assert.Throws<InvalidExpenseCategoryNameException>(() =>
                category.ChangeName(null!));
        }
    }
}
