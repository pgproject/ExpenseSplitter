using ExpenseSplitter.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseSplitter.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                .IsRequired();

            builder.Property(x => x.PasswordHash)
                .IsRequired();

            builder.OwnsOne(x => x.UserSettings, settings =>
            {
                settings.Property(x => x.IsAdmin)
                    .IsRequired();

                settings.Property(x => x.CanCreateExpenseCategory)
                    .IsRequired();

                settings.Property(x => x.CanEditExpenseCategory)
                    .IsRequired();

                settings.Property(x => x.CanRemoveExpenseCategory)
                    .IsRequired();

                settings.Property(x => x.DefaultUserCurrency)
                    .IsRequired();
            });
        }
    }
}
