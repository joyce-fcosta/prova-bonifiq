using ProvaPub.Enums;
using ProvaPub.Models;

namespace ProvaPub.Interfaces
{
    public interface IOrderService
    {
        Task<Order> PayOrderAsync(PaymentMethod paymentMethod, decimal paymentValue, int customerId);
        Task<Order> InsertOrderAsync(Order order);
    }
}
