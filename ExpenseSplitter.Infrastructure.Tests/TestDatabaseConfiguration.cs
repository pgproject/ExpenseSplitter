using Microsoft.Extensions.Configuration;

namespace ExpenseSplitter.Infrastructure.Tests
{
    public static class TestDatabaseConfiguration
    {
        private static readonly IConfiguration Configuration =
            new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

        public static string ConnectionString =>
            Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Test database connection string is not configured.");
    }
}
