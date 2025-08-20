using ProvaPub.Enums;
using ProvaPub.Interfaces;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class OrderService : IOrderService
    {
        private readonly TestDbContext _ctx;
        private readonly Dictionary<PaymentMethod, IPaymentService> _strategies;

        public OrderService(TestDbContext ctx, IEnumerable<IPaymentService> strategies)
        {
            _ctx = ctx;
            _strategies = strategies.ToDictionary(s => s.Method, s => s);
        }

        public async Task<Order> PayOrderAsync(PaymentMethod paymentMethod, decimal paymentValue, int customerId)
        {
            if (!_strategies.TryGetValue(paymentMethod, out var strategy))
            {
                throw new InvalidOperationException("Forma de pagamento não suportada.");
            }

            await strategy.PayAsync(paymentValue, customerId);

            var order = Order.Create(paymentValue, customerId);

            return await InsertOrderAsync(order);

        }

        public async Task<Order> InsertOrderAsync(Order order)
        {
            await _ctx.Orders.AddAsync(order);
            await _ctx.SaveChangesAsync();
            return order;
        }
    }
}
