using FitBridge_Domain.Entities.MessageAndReview;

namespace FitBridge_Application.Specifications.Notifications.GetNotificationsByUserId
{
    public class GetNotificationsByUserIdSpecification : BaseSpecification<Notification>
    {
        public GetNotificationsByUserIdSpecification(
            Guid userId, GetNotificationsByUserIdParams parameters) : base(x =>
            x.IsEnabled && x.UserId == userId)
        {
            if (parameters.DoApplyPaging)
            {
                AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size);
            }
        }
    }
}