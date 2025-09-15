namespace FitBridge_Domain.Exceptions
{
    public class CreateFailedException(string resourceName) : Exception($"Create resource {resourceName} failed")
    {
    }
}