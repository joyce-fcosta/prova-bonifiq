using ProvaPub.Models;

namespace ProvaPub.Interfaces
{
    public interface ICustomerService
    {
        Task<PagedList<Customer>> ListCustomersAsync(int page);
        Task<bool> CanPurchaseAsync(int customerId, decimal purchaseValue);
    }
}
