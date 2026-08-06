using ExpenseSplitter.Domain.Entities.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseSplitter.Infrastructure.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.OwnsOne(x => x.Settings, settings =>
            {
                settings.Property(x => x.OnlyOwnerCanAddMember);
                settings.Property(x => x.OnlyOwnerCanRemoveMember);
            });
        }
    }
}
