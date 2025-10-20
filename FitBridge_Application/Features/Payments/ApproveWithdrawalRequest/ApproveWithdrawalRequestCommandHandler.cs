using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Notifications;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Application.Specifications.Payments.GetWithdrawalRequestByUserIdSpec;
using FitBridge_Domain.Entities.Identity;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Payments.ApproveWithdrawalRequest
{
    internal class ApproveWithdrawalRequestCommandHandler(
        IUnitOfWork unitOfWork,
        IUserUtil userUtil,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<ApproveWithdrawalRequestCommand>
    {
        public async Task Handle(ApproveWithdrawalRequestCommand request, CancellationToken cancellationToken)
        {
            var accountId = userUtil.GetAccountId(httpContextAccessor.HttpContext)
                ?? throw new NotFoundException(nameof(ApplicationUser));

            var spec = new GetWithdrawalRequestByUserIdSpec(accountId);

            var withdrawalRequest = await unitOfWork.Repository<WithdrawalRequest>()
                .GetBySpecificationAsync(spec, asNoTracking: false)
                ?? throw new NotFoundException(nameof(WithdrawalRequest));

            if (withdrawalRequest.Status != WithdrawalRequestStatus.AdminApproved)
            {
                throw new InvalidDataException($"Cannot approve withdrawal request with status: " +
                    $"{withdrawalRequest.Status}. Only admin-approved requests can be user-approved.");
            }

            UpdateWithdrawalRequestStatus(withdrawalRequest);

            await unitOfWork.CommitAsync();
        }

        private void UpdateWithdrawalRequestStatus(WithdrawalRequest withdrawalRequest)
        {
            withdrawalRequest.Status = WithdrawalRequestStatus.Resolved;
            withdrawalRequest.UpdatedAt = DateTime.UtcNow;
            withdrawalRequest.IsUserApproved = true;
            unitOfWork.Repository<WithdrawalRequest>().Update(withdrawalRequest);
        }
    }
}