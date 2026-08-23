using CodeTrail.Application.Auth;
using CodeTrail.Domain.Entities;
using Identity = Microsoft.AspNetCore.Identity;

namespace CodeTrail.Infrastructure.Auth;

public class PasswordHasherAdapter : IPasswordHasher
{
    // PasswordHasher<TUser> ignores the user instance passed to it (it's only there
    // for extensibility hooks we don't use), so a shared null-user instance is safe.
    private readonly Identity.PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(null!, password);

    public PasswordVerificationResult Verify(string passwordHash, string providedPassword)
    {
        Identity.PasswordVerificationResult result;

        try
        {
            result = _hasher.VerifyHashedPassword(null!, passwordHash, providedPassword);
        }
        catch (FormatException)
        {
            // A stored hash that isn't valid base64 can't possibly match - treat it as a
            // failed verification instead of letting the exception bubble up as a 500.
            return PasswordVerificationResult.Failed;
        }

        return result switch
        {
            Identity.PasswordVerificationResult.Success => PasswordVerificationResult.Success,
            Identity.PasswordVerificationResult.SuccessRehashNeeded => PasswordVerificationResult.SuccessRehashNeeded,
            _ => PasswordVerificationResult.Failed
        };
    }
}
