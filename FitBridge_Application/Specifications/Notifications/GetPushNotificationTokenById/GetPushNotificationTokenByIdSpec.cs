using FitBridge_Domain.Entities.MessageAndReview;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Notifications.GetPushNotificationTokenById
{
    public class GetPushNotificationTokenByIdSpec : BaseSpecification<PushNotificationTokens>
    {
        public GetPushNotificationTokenByIdSpec(Guid id) : base(x => x.Id == id)
        {
        }
    }
}