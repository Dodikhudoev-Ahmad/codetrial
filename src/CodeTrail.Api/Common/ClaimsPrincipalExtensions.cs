using System.Security.Claims;

namespace CodeTrail.Api.Common;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("The current principal does not have a NameIdentifier claim.");

        return Guid.Parse(value);
    }
}
