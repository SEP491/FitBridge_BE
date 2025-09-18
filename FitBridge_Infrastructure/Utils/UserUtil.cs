using System;
using FitBridge_Application.Interfaces.Utils;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FitBridge_Infrastructure.Utils;

public class UserUtil : IUserUtil
{
    public Guid? GetAccountId(HttpContext httpContext)
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
