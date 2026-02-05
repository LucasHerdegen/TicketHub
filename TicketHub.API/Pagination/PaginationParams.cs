namespace TicketHub.API.Pagination
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;

        private int _pageNumber { get; set; } = 1;
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = (value >= 1) ? value : _pageNumber;
        }

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value >= 1 && value < MaxPageSize) ? value : MaxPageSize / 2;
        }
    }

}