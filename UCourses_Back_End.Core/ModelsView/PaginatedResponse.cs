namespace UCourses_Back_End.Core.ModelsView
{
    public class PaginatedResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string? WordForSearch { get; set; }
        public string? SortBy { get; set; }
        public IEnumerable<T> Data { get; set; } = new List<T>();

        public PaginatedResponse(IEnumerable<T> data, int totalCount, QueryParams queryParams)
        {
            Data = data;
            TotalCount = totalCount;
            PageNumber = queryParams.PageNumber;
            PageSize = queryParams.PageSize;
            WordForSearch = queryParams.WordForSearch;
            SortBy = queryParams.SortBy;
            TotalPages = (int)Math.Ceiling(totalCount / (double)queryParams.PageSize);
        }
    }
}
