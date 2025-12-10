using FitBridge_Domain.Entities.Orders;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Specifications.Dashboards.GetTransactionForAvailableBalanceDetail
{
    public class GetTransactionForAvailableBalanceDetailSpec : BaseSpecification<Transaction>
    {
        public GetTransactionForAvailableBalanceDetailSpec(Guid userId, GetAvailableBalanceDetailParams parameters) : base(x =>
            (x.TransactionType == TransactionType.DistributeProfit || x.TransactionType == TransactionType.Withdraw)
            && x.WalletId == userId
            // Filter by transaction type if specified
            && (!parameters.TransactionType.HasValue || x.TransactionType == parameters.TransactionType.Value)
            // Filter by date range - From date
            && (!parameters.From.HasValue || x.CreatedAt >= parameters.From.Value)
            // Filter by date range - To date (inclusive, end of day)
            && (!parameters.To.HasValue || x.CreatedAt <= parameters.To.Value.Date.AddDays(1).AddTicks(-1))
            // Search by gym course name or freelance PT package name
            && (string.IsNullOrEmpty(parameters.SearchTerm)
                || (x.OrderItem != null && x.OrderItem.GymCourse != null && x.OrderItem.GymCourse.Name.ToLower().Contains(parameters.SearchTerm.ToLower()))
                || (x.OrderItem != null && x.OrderItem.FreelancePTPackage != null && x.OrderItem.FreelancePTPackage.Name.ToLower().Contains(parameters.SearchTerm.ToLower()))))
        {
            AddInclude(x => x.Order!.OrderItems);
            AddInclude("OrderItem.GymCourse");
            AddInclude("OrderItem.FreelancePTPackage");
            AddInclude("OrderItem.Order");
            AddInclude(x => x.WithdrawalRequest);

            AddOrderBy(x => x.CreatedAt);

            if (parameters.DoApplyPaging)
            {
                AddPaging((parameters.Page - 1) * parameters.Size, parameters.Size);
            }
            else
            {
                parameters.Size = -1;
                parameters.Page = -1;
            }
        }
    }
}