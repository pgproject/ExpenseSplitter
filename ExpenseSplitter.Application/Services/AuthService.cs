using ExpenseSplitter.Application.DTOs.Authentication;
using ExpenseSplitter.Domain.Exceptions;
using ExpenseSplitter.Domain.Entities;
using ExpenseSplitter.Application.Interfaces.Authentication;
using ExpenseSplitter.Application.Interfaces.Users;

namespace ExpenseSplitter.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
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

            User newUser = new User(email: request.Email, passwordHash: _passwordHasher.HashPassword(request.Password));
            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();
            return new RegisterResponse(newUser.Id);
        }

        public async Task<LoginResponse> LoginUserAsync(LoginRequest request)
        {
            User? user = await _userRepository.FindByEmailAsync(request.Email) ?? throw new InvalidCredentialsException();

            if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
            {
                throw new InvalidCredentialsException();
            }

            string accessToken = _jwtTokenGenerator.Genereate(user);

            return new LoginResponse(user.Id, user.Email, user.Role, accessToken);
        }
    }
}
