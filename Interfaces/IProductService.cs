using ProvaPub.Models;

namespace ProvaPub.Interfaces
{
    public interface IProductService
    {
        Task<PagedList<Product>> ListProductsAsync(int page);
    }
}
