namespace ExpenseSplitter.Domain.Entities.User
{
    public class UserSettings
    {
        public bool IsAdmin { get; set; }
        public bool CanCreateExpanseCategory { get; private set; }
        public bool CanEditExpenseCategory { get; private set; }
        public bool CanRemoveExpenseCategory { get; private set; }

        public UserSettings(bool isAdmin, bool canCreateExpanseCategory, bool canEditExpenseCategory, bool canRemoveExpenseCategory)
        {
            IsAdmin = isAdmin;
            CanCreateExpanseCategory = canCreateExpanseCategory;
            CanEditExpenseCategory = canEditExpenseCategory;
            CanRemoveExpenseCategory = canRemoveExpenseCategory;
        }

        public void ChangeUserSettings(UserSettings newSettings)
        {
            IsAdmin = newSettings.IsAdmin;
            CanCreateExpanseCategory = newSettings.CanCreateExpanseCategory;
            CanEditExpenseCategory = newSettings.CanEditExpenseCategory;
            CanRemoveExpenseCategory = newSettings.CanRemoveExpenseCategory;
        }
    }
}
