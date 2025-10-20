namespace FitBridge_Domain.Exceptions
{
    public class InsufficientWalletBalanceException(decimal availableBalance,
        decimal requiredAmount) : BusinessException(
            $"Insufficient available balance. Available: {availableBalance}, Required: {requiredAmount}")
    {
    }
}