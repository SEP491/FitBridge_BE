using FitBridge_Domain.Entities.Orders;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Payments.GetWithdrawalRequestByUserIdSpec
{
    public class GetWithdrawalRequestByUserIdSpec : BaseSpecification<WithdrawalRequest>
    {
        public GetWithdrawalRequestByUserIdSpec(Guid accountId) : base(x =>
            x.IsEnabled
            && x.AccountId == accountId)
        {
        }
    }
}