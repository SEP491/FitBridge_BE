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

    public string? GetUserRole(HttpContext httpContext)
    {
        if (httpContext.User == null)
        {
            return null;
        }

        // The role is stored as ClaimTypes.Role in the access token
        var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role);
        if (roleClaim == null)
        {
            return null;
        }

        // If multiple roles, get the first one
        var roles = roleClaim.Value.Split(',');
        return roles.FirstOrDefault();
    }

    public string? GetUserFullName(HttpContext httpContext)
    {
        if (httpContext.User == null)
        {
            return null;
        }

        var nameClaim = httpContext.User.FindFirst(ClaimTypes.Name);
        if (nameClaim == null)
        {
            return null;
        }

        var roles = nameClaim.Value.Split(',');
        return roles.FirstOrDefault();
    }
}