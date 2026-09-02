using ExpenseSplitter.Domain.Exceptions.ExpenseCategory;

namespace ExpenseSplitter.Domain.Entities
{
    public class ExpenseCategory
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public Guid? UserId { get; private set; }
        public bool IsPublic {
            get
            {
                return UserId == null;
            }
        } 

        private ExpenseCategory() 
        {
        }

        public ExpenseCategory(string name, Guid? userId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidExpenseCategoryNameException();
            }

            Id = Guid.NewGuid();
            Name = name;
            UserId = userId;
        }

        public void ChangeName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new InvalidExpenseCategoryNameException();
            }

            if (IsPublic)
            {
                throw new OnlyPrivateExpenseCategoryCanBeChangedException();
            }
            
            Name = newName;
        }
    }
}
