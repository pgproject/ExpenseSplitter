using ExpenseSplitter.Domain.Enums;

namespace ExpenseSplitter.Domain.Entities.Groups
{
    public class GroupSettings
    {
        public bool OnlyOwnerCanAddMember { get; private set; } = false;
        public bool OnlyOwnerCanRemoveMember { get; private set; } = false;

        public Currency DefaultCurrency { get; private set; }

        private GroupSettings()
        {
        }

        public GroupSettings(Currency defaultCurrency)
        {
            SetDefaultCurrency(defaultCurrency);
        }

        public void SetDefaultCurrency(Currency currency)
        {
            DefaultCurrency = currency;
        }

        public void ChangeSettings(bool onlyOwnerCanAddMember, bool onlyOwnerCanRemoveMember, Currency newDefaultCurrency)
        {
            OnlyOwnerCanAddMember = onlyOwnerCanAddMember;
            OnlyOwnerCanRemoveMember = onlyOwnerCanRemoveMember;
            DefaultCurrency = newDefaultCurrency;
        }
    }
}
