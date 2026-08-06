using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ExpenseSplitter.Application.Interfaces.Authentication;
using ExpenseSplitter.Application.Interfaces.Users;
using ExpenseSplitter.Infrastructure.Authentication;
using ExpenseSplitter.Infrastructure.Data;
using ExpenseSplitter.Infrastructure.Repositories;
using ExpenseSplitter.Infrastructure.Security;
using System.Text;

namespace ExpenseSplitter.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql
            (
                configuration.GetConnectionString("DefaultConnection"))
            );

            services.Configure<JwtSettings>(
                configuration.GetSection("Jwt"));

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, IdentityPasswordHasher>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration
                .GetSection("Jwt")
                .Get<JwtSettings>()
                ?? throw new InvalidOperationException("JWT settings are missing");

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }
}
