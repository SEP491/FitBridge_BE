namespace FitBridge_Application.Specifications
{
    public abstract class BaseParams
    {
        private const int _maxPageSize = 20;

        public static int PAGE_SIZE = 10;

        public bool DoApplyPaging { get; set; } = true;

        public int Page { get; set; } = 1;

        public int Size
        {
            get { return PAGE_SIZE; }
            set { PAGE_SIZE = value > _maxPageSize ? _maxPageSize : value; }
        }

        public string SortBy { get; set; } = "Id";

        public string SortOrder { get; set; } = "asc";

        public string SearchTerm { get; set; } = string.Empty;
    }
}