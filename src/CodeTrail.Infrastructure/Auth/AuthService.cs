using CodeTrail.Application.Auth;
using CodeTrail.Application.Auth.Dtos;
using CodeTrail.Application.Auth.Exceptions;
using CodeTrail.Domain.Entities;
using CodeTrail.Domain.Enums;
using CodeTrail.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeTrail.Infrastructure.Auth;

public class AuthService(
    CodeTrailDbContext db,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator tokenGenerator) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var email = NormalizeEmail(request.Email);

        if (await db.Users.AnyAsync(u => u.Email == email))
        {
            throw new EmailAlreadyInUseException(email);
        }

        var user = new User
        {
            Email = email,
            PasswordHash = passwordHasher.Hash(request.Password),
            DisplayName = request.DisplayName.Trim(),
            Role = UserRole.Student,
            TotalXp = 0,
            CurrentStreak = 0,
            LastActivityDate = null,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Covers the race where two registrations for the same email land concurrently
            // and both pass the AnyAsync check above before either commits.
            throw new EmailAlreadyInUseException(email);
        }

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var email = NormalizeEmail(request.Email);
        var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email);

        if (user is null)
        {
            throw new InvalidCredentialsException();
        }

        var verification = passwordHasher.Verify(user.PasswordHash, request.Password);

        if (verification == PasswordVerificationResult.Failed)
        {
            throw new InvalidCredentialsException();
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.Hash(request.Password);
            await db.SaveChangesAsync();
        }

        return BuildAuthResponse(user);
    }

    public async Task<UserDto> GetCurrentUserAsync(Guid userId)
    {
        var user = await db.Users.FindAsync(userId)
            ?? throw new UserNotFoundException(userId);

        return MapToDto(user);
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var (token, expiresAt) = tokenGenerator.Generate(user);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = MapToDto(user)
        };
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        DisplayName = user.DisplayName,
        Role = user.Role,
        TotalXp = user.TotalXp,
        CurrentStreak = user.CurrentStreak,
        LastActivityDate = user.LastActivityDate,
        CreatedAt = user.CreatedAt
    };
}
