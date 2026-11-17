using ExpenseSplitter.Application.DTOs.Authentication;
using ExpenseSplitter.Application.Interfaces.Authentication;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using ExpenseSplitter.Domain.Exceptions.Authentication;

namespace ExpenseSplitter.Infrastructure.Authentication
{
    public class GoogleTokenValidator : IGoogleTokenValidator
    {
        private readonly string _clientId;

        public GoogleTokenValidator(IConfiguration configuration)
        {
            _clientId = configuration["Google:ClientId"]
                ?? throw new InvalidOperationException(
                    "Google Client ID is not configured.");
        }

        public async Task<GoogleUserInfo> ValidateAsync(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _clientId }
            };

            GoogleJsonWebSignature.Payload payload;

            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch (InvalidJwtException)
            {
                throw new InvalidCredentialsException();
            }

            if (string.IsNullOrWhiteSpace(payload.Subject) || string.IsNullOrWhiteSpace(payload.Email))
            {
                throw new InvalidCredentialsException();
            }

            return new GoogleUserInfo
            (
                payload.Subject,
                payload.Email,
                payload.EmailVerified
            );
        }
    }
}
