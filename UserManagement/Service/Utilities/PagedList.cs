using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Utilities
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(List<T> list, int count, int pageIndex, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(list);
        }
    }

    public static class PagedExtensions
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> query,
            int pageIndex, int pageSize) where T : class
        {
            var count = query.Count();
            var items = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<T>(items, count, pageIndex, pageSize); ;
        }
    }
}
