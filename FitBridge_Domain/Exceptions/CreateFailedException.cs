namespace FitBridge_Domain.Exceptions
{
    public class CreateFailedException(string resourceName) : BusinessException($"Create resource {resourceName} failed")
    {
    }
}