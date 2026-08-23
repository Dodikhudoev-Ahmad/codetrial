using CodeTrail.Application.Auth.Dtos;

namespace CodeTrail.Application.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    Task<AuthResponse> LoginAsync(LoginRequest request);

    Task<UserDto> GetCurrentUserAsync(Guid userId);
}
