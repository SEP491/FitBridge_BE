namespace FitBridge_Domain.Exceptions
{
    public class DuplicateUserException(string message) : BusinessException(message)
    {
    }
}