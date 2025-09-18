namespace FitBridge_API.Helpers.RequestHelpers
{
    public class BaseResponse<T>(string status, string? message, T? data)
    {
        public string Status { get; set; } = status;

        public string? Message { get; set; } = message;

        public T? Data { get; set; } = data;
    }
}