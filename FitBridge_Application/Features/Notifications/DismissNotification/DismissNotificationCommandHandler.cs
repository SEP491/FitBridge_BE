using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Exceptions;
using MediatR;

namespace FitBridge_Application.Features.Notifications.DismissNotification
{
    internal class DismissNotificationCommandHandler(
        IUnitOfWork unitOfWork) : IRequestHandler<DismissNotificationCommand>
    {
        public async Task Handle(DismissNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = await unitOfWork.Repository<Notification>().GetByIdAsync(request.Id)
                ?? throw new NotFoundException(nameof(Notification));

            unitOfWork.Repository<Notification>().SoftDelete(notification);

            await unitOfWork.CommitAsync();
        }
    }
}