using System;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Interfaces.Utils;

public interface IUserUtil
{
    string? GetAvatarUrl(HttpContext httpContext);

    Guid? GetAccountId(HttpContext httpContext);

    string? GetUserRole(HttpContext httpContext);

    string? GetUserFullName(HttpContext httpContext);
}