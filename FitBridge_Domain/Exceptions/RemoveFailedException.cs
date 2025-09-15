namespace FitBridge_Domain.Exceptions
{
    public class RemoveFailedException : BusinessException
    {
        public RemoveFailedException(string resourceName)
            : base($"Remove resource {resourceName} failed")
        {
        }

        public RemoveFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}