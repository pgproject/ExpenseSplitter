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

            builder.HasMany(x => x.Members)
                .WithOne()
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(x => x.Members)
                .HasField("_members")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
