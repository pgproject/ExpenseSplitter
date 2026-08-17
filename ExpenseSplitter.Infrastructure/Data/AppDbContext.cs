using ExpenseSplitter.Domain.Entities.Expense;
using ExpenseSplitter.Domain.Entities.Groups;
using ExpenseSplitter.Domain.Entities.User;
using ExpenseSplitter.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ExpenseSplitter.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<ExpenseCategory> ExpenseCategory { get; set; } = null!;

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GroupMemberConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GroupConfiguration).Assembly);
        }
    }
}
