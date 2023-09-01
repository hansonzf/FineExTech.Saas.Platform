namespace LocationApi.Payload
{
    public class PaginatedItemsPayload<T>
        where T : class
    {
        public bool Result { get; set; }
        public IEnumerable<T> Data { get; set; }
        public string Message { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
