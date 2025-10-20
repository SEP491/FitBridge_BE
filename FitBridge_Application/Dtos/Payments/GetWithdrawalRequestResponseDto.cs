using FitBridge_Domain.Enums.Orders;

namespace FitBridge_Application.Dtos.Payments
{
    public class GetWithdrawalRequestResponseDto
    {
        public Guid Id { get; set; }
        public WithdrawalRequestStatus Status { get; set; }
        public decimal Amount { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string ImageUrl { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid AccountId { get; set; }
        public string AccountFullName { get; set; }
    }
}
