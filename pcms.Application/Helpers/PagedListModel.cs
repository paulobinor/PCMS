namespace pcms.Application.Helpers
{
    public class PagedListModel<T>
    {
        public int TotalItems { get; set; } = 0;
        public int TotalPages { get; set; } = 0;
        public int PageSize { get; set; } = 1;
        public int PageNumber { get; set; } = 1;
        public List<T> Items { get; set; }
    }
}
