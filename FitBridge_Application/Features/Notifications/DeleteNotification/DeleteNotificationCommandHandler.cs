using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Notifications.DeleteNotification
{
    internal class DeleteNotificationCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<DeleteNotificationCommand>
    {
        public async Task Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = await unitOfWork.Repository<Notification>()
                .GetByIdAsync(request.Id, asNoTracking: false) 
                ?? throw new NotFoundException(nameof(Notification));

            unitOfWork.Repository<Notification>().SoftDelete(notification);
            await unitOfWork.CommitAsync();
        }
    }
}