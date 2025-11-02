using FitBridge_Application.Commons.Utils;
using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Dtos.Templates;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services.Notifications;
using FitBridge_Application.Specifications.Wallets.GetWalletByUserId;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Enums.Orders;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Payments.ApproveWithdrawalRequest
{
    internal class ApproveWithdrawalRequestCommandHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService) : IRequestHandler<ApproveWithdrawalRequestCommand>
    {
        public async Task Handle(ApproveWithdrawalRequestCommand request, CancellationToken cancellationToken)
        {
            var withdrawalRequest = await unitOfWork.Repository<WithdrawalRequest>()
                .GetByIdAsync(request.WithdrawalRequestId, asNoTracking: false)
                ?? throw new NotFoundException(nameof(WithdrawalRequest));

            if (withdrawalRequest.Status != WithdrawalRequestStatus.Pending)
            {
                throw new DataValidationFailedException($"Cannot approve withdrawal request with status: " +
                    $"{withdrawalRequest.Status}. Only pending requests can be approved.");
            }

            UpdateWithdrawalRequest(withdrawalRequest);

            var updatedWallet = await UpdateUserWallet(withdrawalRequest);
            await InsertTransactionAsync(withdrawalRequest, updatedWallet);

            await unitOfWork.CommitAsync();
            await SendNotification(withdrawalRequest);
        }

        private async Task<Wallet> UpdateUserWallet(WithdrawalRequest withdrawalRequest)
        {
            var spec = new GetWalletByUserIdSpec(withdrawalRequest.AccountId);
            var wallet = await unitOfWork.Repository<Wallet>()
                .GetBySpecificationAsync(spec, asNoTracking: false)
                ?? throw new NotFoundException(nameof(Wallet));

            wallet.AvailableBalance -= withdrawalRequest.Amount;

            unitOfWork.Repository<Wallet>().Update(wallet);

            return wallet;
        }

        private async Task InsertTransactionAsync(WithdrawalRequest withdrawalRequest, Wallet updatedWallet)
        {
            var transaction = new Transaction
            {
                Status = TransactionStatus.Success,
                Amount = withdrawalRequest.Amount,
                OrderCode = GenerateOrderCode(),
                Description = $"Withdrawal request approved - Amount: {withdrawalRequest.Amount}",
                TransactionType = TransactionType.Withdraw,
                WalletId = updatedWallet.Id,
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

        private void UpdateWithdrawalRequest(WithdrawalRequest withdrawalRequest)
        {
            withdrawalRequest.Status = WithdrawalRequestStatus.AdminApproved;
            withdrawalRequest.IsUserApproved = false;
            withdrawalRequest.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Repository<WithdrawalRequest>().Update(withdrawalRequest);
        }

        private static long GenerateOrderCode()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}