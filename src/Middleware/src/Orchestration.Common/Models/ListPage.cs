using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orchestration.Common.Models
{
    public static class ListPage
    {
        public const int DEFAULT_PAGE_SIZE = 100;
        public const int MAX_PAGE_SIZE = 10000;
    }

    public class ListPage<T>
    {
        public ListPageMeta Meta { get; set; }
        public IList<T> Items { get; set; }
        public ListPage<T2> Select<T2>(Func<T, T2> convert)
        {
            return new ListPage<T2>
            {
                Meta = this.Meta,
                Items = this.Items.Select(convert).ToList()
            };
        }
    }

    public class ListPageMeta
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (TotalCount == 0 ? 0 : (TotalCount + PageSize - 1) / PageSize);

        public int[] ItemRange
        {
            get
            {
                var min = PageSize * (Page - 1) + 1;
                var max = Math.Min(min + PageSize - 1, TotalCount);
                return new[] { min, max };
            }
        }
    }
}
