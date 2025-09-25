using FitBridge_Domain.Entities.MessageAndReview;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Notifications
{
    public class GetDeviceTokenByUserSpecification : BaseSpecification<PushNotificationTokens>
    {
        public GetDeviceTokenByUserSpecification(Guid userId) : base(x =>
            x.UserId == userId)
        {
        }
    }
}