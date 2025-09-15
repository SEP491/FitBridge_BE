namespace FitBridge_Application.Dtos
{
    public class BaseResponse<T>(string status, string? message, T? data) where T : class
    {
        public string Status { get; set; } = status;

        public string? Message { get; set; } = message;

        public T? Data { get; set; } = data;
    }
}