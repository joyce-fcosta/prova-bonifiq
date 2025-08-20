using ProvaPub.Enums;
using ProvaPub.Interfaces;
using ProvaPub.Models;

namespace ProvaPub.Services.Payments
{
    public abstract class BasePaymentService : IPaymentService
    {
        public abstract PaymentMethod Method { get; }

        public virtual async Task<Order> PayAsync(decimal paymentValue, int customerId)
        {
            return await Task.FromResult(Order.Create(paymentValue, customerId));
        }
    }
}
