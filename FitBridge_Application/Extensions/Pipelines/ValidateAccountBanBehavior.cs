using FitBridge_Application.Features.Identities.Login;
using FitBridge_Application.Features.Identities.Registers.RegisterAccounts;
using FitBridge_Application.Features.Identities.Token;
using FitBridge_Application.Features.Notifications.AddUserDeviceToken;
using FitBridge_Application.Features.Payments.PaymentCallbackWebhook;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Services;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Extensions.Pipelines
{
    internal class ValidateAccountBanBehavior<TRequest, TResponse>(
        AccountService accountService,
        IHttpContextAccessor httpContextAccessor,
        IUserUtil userUtil) : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (request is LoginUserCommand
                or RefreshTokenCommand
                or RegisterAccountCommand
                or AddUserDeviceTokenCommand
                or PaymentCallbackWebhookCommand)
            {
                return await next(cancellationToken);
            }

            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                    ?? throw new NotFoundException(nameof(ApplicationUser));
            await accountService.ValidateIsBanned(accountId);
            var response = await next(cancellationToken);

            return response;
        }
    }
}