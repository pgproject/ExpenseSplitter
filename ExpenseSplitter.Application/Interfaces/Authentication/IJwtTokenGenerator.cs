using ExpenseSplitter.Domain.Entities;

namespace ExpenseSplitter.Application.Interfaces.Authentication
{
    public interface IJwtTokenGenerator
    {
        string Genereate(User user);
    }
}
