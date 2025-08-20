using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;

namespace ProvaPub.Helpers
{
    public static class PaginationHelper
    {
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> query, int page, int pageSize = 10)
        {
            var totalCount = await query.CountAsync(); // Use CountAsync()
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(); // Use ToListAsync()

            return new PagedList<T>
            {
                Items = items,
                TotalCount = totalCount,
                HasNext = totalCount > page * pageSize
            };
        }
    }
}
