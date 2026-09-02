using ExpenseSplitter.Domain.Enums;
using ExpenseSplitter.Domain.Exceptions.User;

namespace ExpenseSplitter.Domain.Entities.User
{
    public class UserSettings
    {
        public bool IsAdmin { get; set; }
        public bool CanCreateExpenseCategory { get; private set; }
        public bool CanEditExpenseCategory { get; private set; }
        public bool CanRemoveExpenseCategory { get; private set; }
        public Currency DefaultUserCurrency { get; private set; }

        public UserSettings(bool isAdmin, bool canCreateExpenseCategory, bool canEditExpenseCategory, bool canRemoveExpenseCategory, Currency currency)
        {
            IsAdmin = isAdmin;
            CanCreateExpenseCategory = canCreateExpenseCategory;
            CanEditExpenseCategory = canEditExpenseCategory;
            CanRemoveExpenseCategory = canRemoveExpenseCategory;
            if (!Enum.IsDefined(currency))
            {
                throw new InvalidUserCurrencyException();
            }
            DefaultUserCurrency = currency;
        }

        public void ChangeUserSettings(UserSettings newSettings)
        {
            IsAdmin = newSettings.IsAdmin;
            CanCreateExpenseCategory = newSettings.CanCreateExpenseCategory;
            CanEditExpenseCategory = newSettings.CanEditExpenseCategory;
            CanRemoveExpenseCategory = newSettings.CanRemoveExpenseCategory;

            if (!Enum.IsDefined(newSettings.DefaultUserCurrency))
            {
                throw new InvalidUserCurrencyException();
            }
            DefaultUserCurrency = newSettings.DefaultUserCurrency;
        }
    }
}
