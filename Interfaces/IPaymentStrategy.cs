using ProvaPub.Enums;
using ProvaPub.Models;

namespace ProvaPub.Interfaces
{
    public interface IPaymentService
    {
        Task<Order> PayAsync(decimal value, int customerId);
        PaymentMethod Method { get; }
    }
}

