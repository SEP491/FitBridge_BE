using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Payments.GetWithdrawalRequestByUserIdSpec
{
    public class GetWithdrawalRequestByUserIdSpec : BaseSpecification<WithdrawalRequest>
    {
        public GetWithdrawalRequestByUserIdSpec(Guid accountId) : base(x =>
            x.IsEnabled
            && x.AccountId == accountId
            && (x.Status == WithdrawalRequestStatus.Pending
            || x.Status == WithdrawalRequestStatus.AdminApproved
            || x.Status == WithdrawalRequestStatus.UserDisapproved))
        {
        }
    }
}