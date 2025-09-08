namespace FitBridge_Application.Dtos
{
    public class PagingResultDto<T>(int total, IReadOnlyList<T> items) where T : class
    {
        public int Total { get; private set; } = total;

        public IReadOnlyList<T> Items { get; private set; } = items;
    }
}