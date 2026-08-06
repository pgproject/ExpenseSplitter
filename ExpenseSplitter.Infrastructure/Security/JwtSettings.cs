
namespace ExpenseSplitter.Infrastructure.Security
{
    public class JwtSettings
    {
        public required string Key { get; init; }
        public required string Issuer { get; init; }
        public required string Audience { get; init; }
        public required int ExpirationInMinutes { get; init; }
    }
}
