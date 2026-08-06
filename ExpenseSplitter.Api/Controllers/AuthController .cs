using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseSplitter.Application.DTOs.Authentication;
using ExpenseSplitter.Application.Interfaces.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ExpenseSplitter.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> RegisterAsync([FromBody] RegisterRequest request)
        {
            RegisterResponse response = await _authService.RegisterUserAsync(request);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request)
        {
            LoginResponse response = await _authService.LoginUserAsync(request);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                Role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }
    }
}
