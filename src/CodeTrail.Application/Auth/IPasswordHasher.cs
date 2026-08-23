namespace CodeTrail.Application.Auth;

public interface IPasswordHasher
{
    string Hash(string password);

    PasswordVerificationResult Verify(string passwordHash, string providedPassword);
}
