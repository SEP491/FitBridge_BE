namespace FitBridge_Domain.Exceptions
{
    public class ExpiredException(string itemId) : BusinessException($"Resource expired {itemId}")
    {
    }
}