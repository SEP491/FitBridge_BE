namespace FitBridge_Domain.Exceptions
{
    public class NotFoundException(string resourceName, object? key = null) : Exception($"Resource '{resourceName}'{(key != null ? $" with key '{key}'" : "")} was not found.")
    {
    }
}