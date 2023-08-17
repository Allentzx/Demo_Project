using System.Security.Claims;

namespace ITC.Data.Utilities
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? AccountID(this ClaimsPrincipal principal) =>
            Guid.TryParse(principal.Claims.SingleOrDefault(cl => cl.Type == "UserId")?.Value, out var id)
                ? id
                : null;

        public static string? Email(this ClaimsPrincipal principal) =>
            principal.Claims.SingleOrDefault(cl => cl.Type == "Email")?.Value;
    }
}
