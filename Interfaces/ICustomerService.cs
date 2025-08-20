using ProvaPub.Models;

namespace ProvaPub.Interfaces
{
    public interface ICustomerService
    {
        Task<PagedList<Customer>> ListCustomersAsync(int page);
        Task<bool> CanPurchase(int customerId, decimal purchaseValue);
    }
}
