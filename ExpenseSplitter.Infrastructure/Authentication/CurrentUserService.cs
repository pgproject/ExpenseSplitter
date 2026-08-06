using Microsoft.AspNetCore.Http;
using ExpenseSplitter.Application.Interfaces.Users;
using System.Security.Claims;

namespace ExpenseSplitter.Infrastructure.Authentication
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?
                    .User
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value;

                if (!Guid.TryParse(value, out Guid userId))
                {
                    throw new UnauthorizedAccessException(value);
                }

                return userId;
            }
        }
    }
}
