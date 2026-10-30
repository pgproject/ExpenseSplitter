using ExpenseSplitter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseSplitter.Infrastructure.Configurations
{
    public class ExpenseAllocationConfiguration : IEntityTypeConfiguration<ExpenseAllocation>
    {
        public void Configure(EntityTypeBuilder<ExpenseAllocation> builder)
        {
            builder.HasKey(x => new { x.ExpenseId, x.GroupId });

            builder.Property(x => x.ParticipantUserIds)
                .HasColumnType("uuid[]")
                .IsRequired();

            builder.HasOne<Expense>()
                .WithMany(x => x.ExpenseAllocations)
                .HasForeignKey(x => x.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}