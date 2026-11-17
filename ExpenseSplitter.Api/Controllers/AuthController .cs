using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseSplitter.Application.DTOs.Authentication;
using ExpenseSplitter.Application.Interfaces.Authentication;
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
        public async Task<ActionResult<RegisterResponse>> RegisterAsync(
            [FromBody] RegisterRequest request)
        {
            RegisterResponse response =
                await _authService.RegisterUserAsync(request);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> LoginAsync(
            [FromBody] LoginRequest request)
        {
            LoginResponse response =
                await _authService.LoginUserAsync(request);

            return Ok(response);
        }

        [HttpPost("google")]
        public async Task<ActionResult<LoginResponse>> LoginWithGoogleAsync(
            [FromBody] GoogleLoginRequest request)
        {
            LoginResponse response =
                await _authService.LoginWithGoogleAsync(request);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("link-google")]
        public async Task<IActionResult> LinkGoogleAccountAsync([FromBody] GoogleLoginRequest request)
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized();
            }

            await _authService.LinkGoogleAccountAsync(userId, request);

            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized();
            }

            bool googleConnected = await _authService.HasGoogleAccountAsync(userId);

            return Ok(new
            {
                Id = userId,
                Email = User.FindFirstValue(ClaimTypes.Email),
                Role = User.FindFirstValue(ClaimTypes.Role),
                GoogleConnected = googleConnected
            });
        }
    }
}
