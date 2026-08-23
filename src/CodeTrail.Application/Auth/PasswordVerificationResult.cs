namespace CodeTrail.Application.Auth;

public enum PasswordVerificationResult
{
    Failed,
    Success,
    SuccessRehashNeeded
}
