namespace FitBridge_Domain.Exceptions
{
    public class BannedException(string username) : BusinessException($"{username} has been banned")
    {
    }
}