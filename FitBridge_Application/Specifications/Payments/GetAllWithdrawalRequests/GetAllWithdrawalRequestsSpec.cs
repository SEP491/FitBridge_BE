using FitBridge_Domain.Entities.Orders;
using System.Linq.Expressions;

namespace FitBridge_Application.Specifications.Payments.GetAllWithdrawalRequests
{
    public class GetAllWithdrawalRequestsSpec : BaseSpecification<WithdrawalRequest>
    {
        public GetAllWithdrawalRequestsSpec(GetAllWithdrawalRequestsParams parameters)
            : base(x => x.IsEnabled &&
                        (!parameters.AccountId.HasValue || x.AccountId == parameters.AccountId.Value))
        {
            AddInclude("Account");

            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                switch (parameters.SortBy.ToLower())
                {
                    case "amount":
                        if (parameters.SortOrder.ToLower() == "desc")
                            AddOrderByDesc(x => x.Amount);
                        else
                            AddOrderBy(x => x.Amount);
                        break;

                    case "status":
                        if (parameters.SortOrder.ToLower() == "desc")
                            AddOrderByDesc(x => x.Status);
                        else
                            AddOrderBy(x => x.Status);
                        break;

                    case "createdat":
                    case "createddate":
                        if (parameters.SortOrder.ToLower() == "desc")
                            AddOrderByDesc(x => x.CreatedAt);
                        else
                            AddOrderBy(x => x.CreatedAt);
                        break;

                    default:
                        AddOrderByDesc(x => x.CreatedAt);
                        break;
                }
            }
            else
            {
                AddOrderByDesc(x => x.CreatedAt);
            }

            if (parameters.DoApplyPaging)
            {
                AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
            }
        }
    }
}