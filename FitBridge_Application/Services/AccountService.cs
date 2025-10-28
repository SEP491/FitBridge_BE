using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Exceptions;

namespace FitBridge_Application.Services
{
    internal class AccountService(IApplicationUserService applicationUserService)
    {
        internal async Task ValidateIsBanned(Guid userId)
        {
            var user = await applicationUserService.GetByIdAsync(userId);
            if (!user.IsActive)
            {
                throw new BannedException(user.FullName);
            }
        }
    }
}