using Microsoft.EntityFrameworkCore;
using ProvaPub.Helpers;
using ProvaPub.Interfaces;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly TestDbContext _ctx;

        private readonly IDateTimeProvider _dateTimeProvider;

        public CustomerService(TestDbContext ctx, IDateTimeProvider dateTimeProvider)
        {
            _ctx = ctx;
            _dateTimeProvider = dateTimeProvider;
        }

    public async Task<PagedList<Customer>> ListCustomersAsync(int page) => await _ctx.Customers.ToPagedListAsync(page);

        public async Task<bool> CanPurchaseAsync(int customerId, decimal purchaseValue)
        {
            if (customerId <= 0) throw new ArgumentOutOfRangeException(nameof(customerId), "Customer ID must be greater than zero.");
            if (purchaseValue <= 0) throw new ArgumentOutOfRangeException(nameof(purchaseValue), "Purchase value must be greater than zero.");

            //Business Rule: Non registered Customers cannot purchase
            var customer = await _ctx.Customers.FindAsync(customerId);
            if (customer == null) throw new InvalidOperationException($"Customer Id {customerId} does not exists");

            //Business Rule: A customer can purchase only a single time per month
            var baseDate = _dateTimeProvider.UtcNow.AddMonths(-1);  
            var ordersInThisMonth = await _ctx.Orders.CountAsync(s => s.CustomerId == customerId && s.OrderDate >= baseDate);
            if (ordersInThisMonth > 0)
                return false;

            //Business Rule: A customer that never bought before can make a first purchase of maximum 100,00
            var haveBoughtBefore = await _ctx.Customers.CountAsync(s => s.Id == customerId && s.Orders.Any());
            if (haveBoughtBefore == 0 && purchaseValue > 100)
                return false;

            //Business Rule: A customer can purchases only during business hours and working days
            var currentUtc = _dateTimeProvider.UtcNow;
            if (currentUtc.Hour < 8 || currentUtc.Hour > 18 || currentUtc.DayOfWeek == DayOfWeek.Saturday || currentUtc.DayOfWeek == DayOfWeek.Sunday)
                return false;


            return true;
        }

    }
}
