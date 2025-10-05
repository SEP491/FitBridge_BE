namespace FitBridge_Application.Dtos.Transactions
{
    public class WithdrawalRequestDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Note { get; set; }
    }
}