namespace ExpenseSplitter.Domain.Entities.Groups
{
    public class GroupSettings
    {
        public bool OnlyOwnerCanAddMember { get; private set; } = false;
        public bool OnlyOwnerCanRemoveMember { get; private set; } = false;

        public void ChangeSettings(bool onlyOnwerCanAddMember, bool onlyOwnerCanRemoveMember)
        {
            OnlyOwnerCanAddMember = onlyOnwerCanAddMember;
            OnlyOwnerCanRemoveMember = onlyOwnerCanRemoveMember;
        }
    }
}
