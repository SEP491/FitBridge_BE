namespace FitBridge_Domain.Exceptions
{
    public class UpdateFailedException(string resourceName) : BusinessException($"Update resource {resourceName} failed")
    {
    }
}