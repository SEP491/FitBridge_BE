using FitBridge_Domain.Entities.Trainings;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.UserGoals.GetUserGoalsByCustomerPurchased
{
    public class GetUserGoalsByCustomerPurchasedSpec : BaseSpecification<UserGoal>
    {
        public GetUserGoalsByCustomerPurchasedSpec(Guid customerPurchasedId) : base(x =>
            x.IsEnabled
            && x.CustomerPurchasedId == customerPurchasedId)
        {
        }
    }
}