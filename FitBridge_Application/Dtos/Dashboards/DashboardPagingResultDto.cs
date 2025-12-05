namespace FitBridge_Application.Dtos.Dashboards
{
    /// <summary>
    /// Paging result with additional summary information for dashboard data
    /// </summary>
    /// <typeparam name="T">The type of items in the result</typeparam>
    public class DashboardPagingResultDto<T> where T : class
    {
        public int Total { get; set; }
        
        public IReadOnlyList<T> Items { get; set; }
        
        public decimal TotalProfitSum { get; set; }

        public DashboardPagingResultDto(int total, IReadOnlyList<T> items, decimal totalProfitSum)
        {
            Total = total;
            Items = items;
            TotalProfitSum = totalProfitSum;
        }
    }
}
