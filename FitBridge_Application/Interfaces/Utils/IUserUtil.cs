using System;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Interfaces.Utils;

public interface IUserUtil
{
    Guid? GetAccountId(HttpContext httpContext);
}
