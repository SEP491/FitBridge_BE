namespace FitBridge_Application.Specifications
{
    public abstract class BaseParams
    {
        private const int _maxPageSize = 20;

        private int _pageSize = 10;

        public bool DoApplyPaging { get; set; } = true;

        public int Page { get; set; } = 1;

        public int Size
        {
            get { return _pageSize; }
            set { _pageSize = value > _maxPageSize ? _maxPageSize : value; }
        }

        public string SortBy { get; set; } = "Id";

        public string SortOrder { get; set; } = "asc";
    }
}