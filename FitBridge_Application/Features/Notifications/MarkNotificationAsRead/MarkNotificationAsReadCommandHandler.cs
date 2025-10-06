using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Notifications.MarkNotificationAsRead
{
    internal class MarkNotificationAsReadCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<MarkNotificationAsReadCommand>
    {
        public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await unitOfWork.Repository<Notification>()
                .GetByIdAsync(request.Id, asNoTracking: false) ?? throw new NotFoundException(nameof(Notification));

            notification.UpdatedAt = DateTime.UtcNow;
            notification.ReadAt = DateTime.Now;

            unitOfWork.Repository<Notification>().Update(notification);

            await unitOfWork.CommitAsync();
        }
    }
}