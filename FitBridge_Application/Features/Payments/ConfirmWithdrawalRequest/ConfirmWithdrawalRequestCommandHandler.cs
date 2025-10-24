using FitBridge_Application.Commons.Utils;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Dtos.Templates;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Notifications;
using FitBridge_Application.Interfaces.Utils;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FitBridge_Application.Features.Payments.ConfirmWithdrawalRequest
{
    internal class ConfirmWithdrawalRequestCommandHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService) : IRequestHandler<ConfirmWithdrawalRequestCommand>
    {
        public async Task Handle(ConfirmWithdrawalRequestCommand request, CancellationToken cancellationToken)
        {
            var withdrawalRequest = await unitOfWork.Repository<WithdrawalRequest>()
                .GetByIdAsync(request.WithdrawalRequestId, asNoTracking: false)
                ?? throw new NotFoundException(nameof(WithdrawalRequest));

            if (withdrawalRequest.Status != WithdrawalRequestStatus.Pending)
            {
                throw new DataValidationFailedException($"Cannot confirm withdrawal request with status: " +
                    $"{withdrawalRequest.Status}. Only pending requests can be confirmed.");
            }

            UpdateWithdrawalRequest(withdrawalRequest, request.ImageUrl);

            await InsertTransactionAsync(withdrawalRequest);

            await unitOfWork.CommitAsync();
            await SendNotification(withdrawalRequest);
        }

        private async Task InsertTransactionAsync(WithdrawalRequest withdrawalRequest)
        {
            var transaction = new Transaction
            {
                Status = TransactionStatus.Success,
                Amount = withdrawalRequest.Amount,
                OrderCode = GenerateOrderCode(),
                Description = $"Withdrawal request confirmed - Amount: {withdrawalRequest.Amount}",
                TransactionType = TransactionType.Withdraw,
                WithdrawalRequestId = withdrawalRequest.Id,
                PaymentMethodId = await GetSystemPaymentMethodId.GetPaymentMethodId(MethodType.System, unitOfWork)
            };

            unitOfWork.Repository<Transaction>().Insert(transaction);
        }

        private async Task SendNotification(WithdrawalRequest withdrawalRequest)
        {
            var model = new WithdrawalRequestAdminApprovedModel
            {
                BodyAmount = withdrawalRequest.Amount,
            };

            var message = new NotificationMessage(
                EnumContentType.WithdrawalRequestAdminApproved,
                [withdrawalRequest.AccountId],
                model);

            await notificationService.NotifyUsers(message);
        }

        private void UpdateWithdrawalRequest(WithdrawalRequest withdrawalRequest, string imageUrl)
        {
            withdrawalRequest.Status = WithdrawalRequestStatus.AdminApproved;
            withdrawalRequest.IsUserApproved = false;
            withdrawalRequest.ImageUrl = imageUrl;
            withdrawalRequest.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Repository<WithdrawalRequest>().Update(withdrawalRequest);
        }

        private static long GenerateOrderCode()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}