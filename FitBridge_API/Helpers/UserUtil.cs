using System;
using System.Security.Claims;
using FitBridge_Application.Interfaces.Utils;

namespace FitBridge_API.Helpers;

public static class UserUtil
{
    public static Guid? GetAccountId(HttpContext httpContext)
    {
        if (httpContext.User == null)
        {
            return null;
        }

        var nameIdentifierClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (nameIdentifierClaim == null)
        {
            return null;
        }

        if (!Guid.TryParse(nameIdentifierClaim.Value, out Guid accountId))
        {
            return null;
        }
        return accountId;
    }
}
