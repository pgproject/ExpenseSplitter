using Microsoft.Extensions.DependencyInjection;
using ExpenseSplitter.Application.Interfaces.Authentication;
using ExpenseSplitter.Application.Services;

namespace ExpenseSplitter.Application.Extensions
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
