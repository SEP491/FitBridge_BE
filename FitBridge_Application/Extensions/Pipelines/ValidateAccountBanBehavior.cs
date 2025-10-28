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
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                    ?? throw new NotFoundException(nameof(ApplicationUser));
            await accountService.ValidateIsBanned(accountId);
            var response = await next(cancellationToken);

            return response;
        }
    }
}