using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pcms.Application.Helpers
{
    public class Utilities
    {
        public static PagedListModel<T> GetPagedList<T>(List<T> t, int pageNumber = 1, int pageSize = 10)
        {
            if (t == null || t.Count == 0)
            {
                return new PagedListModel<T>();
            }

            var pagedRes = new PagedListModel<T>
            {
                TotalItems = t.Count,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(t.Count / (double)pageSize),
                Items = t.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
            };
            return pagedRes;
        }
    }
}
