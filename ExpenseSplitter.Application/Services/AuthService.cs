using ExpenseSplitter.Application.DTOs.Authentication;
using ExpenseSplitter.Application.Interfaces.Authentication;
using ExpenseSplitter.Application.Interfaces.Users;
using ExpenseSplitter.Domain.Exceptions.Authentication;
using ExpenseSplitter.Domain.Entities.User;

namespace ExpenseSplitter.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IGoogleTokenValidator _googleTokenValidator;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IGoogleTokenValidator googleTokenValidator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _googleTokenValidator = googleTokenValidator;
        }

        public async Task<RegisterResponse> RegisterUserAsync(RegisterRequest request)
        {
            if (request.Password != request.ConfirmPassword)
            {
                throw new PasswordsDoNotMatchException();
            }

            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new EmailAlreadyExistsException();
            }

            User newUser = new User(
                email: request.Email,
                passwordHash: _passwordHasher.HashPassword(request.Password));

            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            return new RegisterResponse(newUser.Id);
        }

        public async Task<LoginResponse> LoginUserAsync(LoginRequest request)
        {
            User? user = await _userRepository.FindByEmailAsync(request.Email)
                ?? throw new InvalidCredentialsException();

            if (user.PasswordHash == null ||
                !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
            {
                throw new InvalidCredentialsException();
            }

            return CreateLoginResponse(user);
        }

        public async Task<LoginResponse> LoginWithGoogleAsync(GoogleLoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.idToken))
            {
                throw new InvalidCredentialsException();
            }

            GoogleUserInfo googleUser =
                await _googleTokenValidator.ValidateAsync(request.idToken);

            if (!googleUser.emailVerified)
            {
                throw new InvalidCredentialsException();
            }

            User? user = await _userRepository.FindByGoogleIdAsync(googleUser.googleId);

            if (user == null)
            {
                // Nie łączymy automatycznie kont na podstawie adresu e-mail.
                if (await _userRepository.ExistsByEmailAsync(googleUser.email))
                {
                    throw new EmailAlreadyExistsException();
                }

                user = new User(
                    email: googleUser.email,
                    passwordHash: null,
                    googleId: googleUser.googleId);

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();
            }

            return CreateLoginResponse(user);
        }

        private LoginResponse CreateLoginResponse(User user)
        {
            string accessToken = _jwtTokenGenerator.Genereate(user);

            return new LoginResponse(
                user.Id,
                user.Email,
                user.Role,
                accessToken);
        }
    }
}
