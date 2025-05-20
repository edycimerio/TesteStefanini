using System.Collections.Generic;

namespace TesteStefanini.Application.DTOs
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;
        
        public int PageNumber { get; set; } = 1;
        
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }

    public class PagedResult<T> where T : class
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<T> Items { get; set; }

        public PagedResult(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)System.Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            Items = items;
        }

        public static PagedResult<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source is ICollection<T> collection ? collection.Count : source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<T>(items, count, pageNumber, pageSize);
        }
    }
}
