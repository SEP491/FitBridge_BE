namespace FitBridge_Domain.Exceptions
{
    public class OutOfStocksException(string resourceName, object? key = null) : BusinessException($"Requested resource'{resourceName}'{(key != null ? $" with key '{key}'" : "")} is out of stocks.")
    {
    }
}