using ExpenseSplitter.Domain.Entities.User;

namespace ExpenseSplitter.Application.Interfaces.Authentication
{
    public interface IJwtTokenGenerator
    {
        string Genereate(User user);
    }
}
