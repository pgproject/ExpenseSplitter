using ExpenseSplitter.Domain.Enums;

namespace ExpenseSplitter.Domain.Entities.Groups
{
    public class GroupSettings
    {
        public bool OnlyOwnerCanAddMember { get; private set; } = false;
        public bool OnlyOwnerCanRemoveMember { get; private set; } = false;

        public Currency DefaultCurrency { get; private set; }

        public void SetDefaultCurrency(Currency currency)
        {
            DefaultCurrency = currency;
        }

        public void ChangeSettings(bool onlyOnwerCanAddMember, bool onlyOwnerCanRemoveMember, Currency newDefaultCurrency)
        {
            OnlyOwnerCanAddMember = onlyOnwerCanAddMember;
            OnlyOwnerCanRemoveMember = onlyOwnerCanRemoveMember;
            DefaultCurrency = newDefaultCurrency;
        }
    }
}
