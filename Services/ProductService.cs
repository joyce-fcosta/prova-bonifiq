using ProvaPub.Helpers;
using ProvaPub.Interfaces;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class ProductService : IProductService
    {
        private readonly TestDbContext _ctx;

        public ProductService(TestDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<PagedList<Product>> ListProductsAsync(int page) => _ctx.Products.ToPagedListAsync(page);
    }
}
