namespace ExpenseSplitter.Domain.Entities.User
{
    public class UserSettings
    {
        public bool IsAdmin { get; set; }
        public bool CanCreateExpanseCategory { get; private set; }
        public bool CanEditExpenseCategory { get; private set; }

        public UserSettings(bool isAdmin, bool canCreateExpanseCategory, bool canEditExpenseCategory)
        {
            IsAdmin = isAdmin;
            CanCreateExpanseCategory = canCreateExpanseCategory;
            CanEditExpenseCategory = canEditExpenseCategory;
        }
    }
}
