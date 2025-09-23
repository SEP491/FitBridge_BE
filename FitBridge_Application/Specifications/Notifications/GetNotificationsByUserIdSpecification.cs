using FitBridge_Domain.Entities.MessageAndReview;

namespace FitBridge_Application.Specifications.Notifications
{
    public class GetNotificationsByUserIdSpecification : BaseSpecification<InAppNotification>
    {
        public GetNotificationsByUserIdSpecification(Guid userId) : base(x => x.UserId == userId && x.IsEnabled)
        {
        }
    }
}