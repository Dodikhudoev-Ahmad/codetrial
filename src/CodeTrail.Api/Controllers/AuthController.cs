using CodeTrail.Api.Common;
using CodeTrail.Application.Auth;
using CodeTrail.Application.Auth.Dtos;
using CodeTrail.Application.Auth.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeTrail.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        try
        {
            return Ok(await authService.RegisterAsync(request));
        }
        catch (EmailAlreadyInUseException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Email already in use",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        try
        {
            return Ok(await authService.LoginAsync(request));
        }
        catch (InvalidCredentialsException ex)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Invalid credentials",
                Detail = ex.Message,
                Status = StatusCodes.Status401Unauthorized
            });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> Me()
    {
        try
        {
            return Ok(await authService.GetCurrentUserAsync(User.GetUserId()));
        }
        catch (UserNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "User not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
    }
}
