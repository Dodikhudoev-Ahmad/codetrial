using CodeTrail.Domain.Entities;

namespace CodeTrail.Application.Auth;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) Generate(User user);
}
