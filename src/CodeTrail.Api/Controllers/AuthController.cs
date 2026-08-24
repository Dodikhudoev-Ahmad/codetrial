using CodeTrail.Api.Common;
using CodeTrail.Application.Auth;
using CodeTrail.Application.Auth.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeTrail.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request) =>
        Ok(await authService.RegisterAsync(request));

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request) =>
        Ok(await authService.LoginAsync(request));

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> Me() =>
        Ok(await authService.GetCurrentUserAsync(User.GetUserId()));
}
