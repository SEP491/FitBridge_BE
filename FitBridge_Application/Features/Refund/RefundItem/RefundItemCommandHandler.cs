using FitBridge_Application.Dtos.Notifications;
using FitBridge_Application.Dtos.Templates;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Interfaces.Services.Notifications;
using FitBridge_Application.Services;
using FitBridge_Application.Specifications.Orders.GetOrderItemById;
using FitBridge_Application.Specifications.Wallets.GetWalletByUserId;
using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;
using Quartz;

namespace FitBridge_Application.Features.Refund.RefundItem
{
    internal class RefundItemCommandHandler(
        IScheduleJobServices scheduleJobServices,
        AccountService accountService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork) : IRequestHandler<RefundItemCommand>
    {
        public async Task Handle(RefundItemCommand request, CancellationToken cancellationToken)
        {
            var spec = new GetOrderItemByIdSpec(request.OrderItemId);
            var existingOrderItem = await unitOfWork.Repository<OrderItem>()
                .GetBySpecificationAsync(spec, asNoTracking: false) ?? throw new NotFoundException(nameof(OrderItem));

            await accountService.ValidateIsBanned(existingOrderItem.Order.AccountId);

            if (existingOrderItem.IsRefunded)
            {
                throw new DataValidationFailedException("Order item has already been refunded");
            }

            var jobName = $"ProfitDistribution_{request.OrderItemId}";
            var jobGroup = "ProfitDistribution";
            var jobStatus = await scheduleJobServices.GetJobStatus(jobName, jobGroup);
            var jobExists = jobStatus != TriggerState.None;

            var refundAmount = existingOrderItem.Price * existingOrderItem.Quantity;

            if (jobExists)
            {
                await scheduleJobServices.CancelScheduleJob(jobName, jobGroup);
            }
            else
            {
                var walletSpec = new GetWalletByUserIdSpec(existingOrderItem.Order.AccountId);
                var wallet = await unitOfWork.Repository<Wallet>()
                    .GetBySpecificationAsync(walletSpec, asNoTracking: false) ?? throw new NotFoundException(nameof(Wallet));

                wallet.AvailableBalance -= refundAmount;
                unitOfWork.Repository<Wallet>().Update(wallet);
            }

            existingOrderItem.IsRefunded = true;

            unitOfWork.Repository<OrderItem>().Update(existingOrderItem);
            await unitOfWork.CommitAsync();

            await NotifyUser(
                refundAmount,
                existingOrderItem.Order.Account.FullName,
                existingOrderItem.Order.AccountId);
        }

        private async Task NotifyUser(decimal refundAmount, string fullName, Guid userId)
        {
            var model = new RefundedItemModel
            {
                BodyAmount = refundAmount,
                BodyRequesterName = fullName
            };

            var message = new NotificationMessage(EnumContentType.RefundedItem, [userId], model);

            await notificationService.NotifyUsers(message);
        }
    }
}