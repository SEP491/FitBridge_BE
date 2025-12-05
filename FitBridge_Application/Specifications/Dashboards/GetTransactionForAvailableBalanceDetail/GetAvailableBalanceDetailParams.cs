using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Specifications.Dashboards.GetTransactionForAvailableBalanceDetail
{
    public class GetAvailableBalanceDetailParams : BaseParams
    {
        private TransactionType? _transactionType;

        /// <summary>
        /// Filter by transaction type. Only DistributeProfit and Withdraw are allowed.
        /// </summary>
        public TransactionType? TransactionType
        {
            get => _transactionType;
            set
            {
                if (value.HasValue && value != FitBridge_Domain.Enums.Orders.TransactionType.DistributeProfit && value != FitBridge_Domain.Enums.Orders.TransactionType.Withdraw)
                {
                    throw new ArgumentException("Only DistributeProfit and Withdraw transaction types are allowed for available balance details.");
                }
                _transactionType = value;
            }
        }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }
    }
}