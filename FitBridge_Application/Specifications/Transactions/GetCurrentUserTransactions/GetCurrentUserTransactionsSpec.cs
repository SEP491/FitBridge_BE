using FitBridge_Application.Commons.Utils;
using FitBridge_Application.Dtos.Transactions;
using FitBridge_Domain.Entities.Orders;

namespace FitBridge_Application.Specifications.Transactions.GetCurrentUserTransactions
{
    public class GetCurrentUserTransactionsSpec : BaseSpecification<Transaction>
    {
        public GetCurrentUserTransactionsSpec(
            GetCurrentUserTransactionsParam parameters,
            Guid ptId,
            bool includeWithdrawRequest = false,
            bool includeOrder = false) : base(x =>
            x.IsEnabled
            && (!includeWithdrawRequest || (includeWithdrawRequest && x.WithdrawalRequest.AccountId == ptId)))
        {
            switch (StringCapitalizationConverter.ToUpperFirstChar(parameters.SortBy))
            {
                case nameof(GetTransactionsDto.Amount):
                    if (parameters.SortOrder == "asc")
                        AddOrderBy(x => x.Amount);
                    else
                        AddOrderByDesc(x => x.Amount);
                    break;

                case nameof(GetTransactionsDto.CreatedAt):
                    if (parameters.SortOrder == "asc")
                        AddOrderBy(x => x.CreatedAt);
                    else
                        AddOrderByDesc(x => x.CreatedAt);
                    break;

                case nameof(GetTransactionsDto.Status):
                    if (parameters.SortOrder == "asc")
                        AddOrderBy(x => x.Status);
                    else
                        AddOrderByDesc(x => x.Status);
                    break;

                default:
                    AddOrderBy(x => x.CreatedAt);
                    break;
            }

            if (parameters.DoApplyPaging)
            {
                AddPaging(parameters.Size * (parameters.Page - 1), parameters.Size);
            }
            else
            {
                parameters.Size = -1;
                parameters.Page = -1;
            }

            if (includeWithdrawRequest)
            {
                AddInclude(x => x.WithdrawalRequest!);
            }
            if (includeOrder)
            {
                AddInclude(x => x.Order!);
            }
        }
    }
}