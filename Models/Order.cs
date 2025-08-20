namespace ProvaPub.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public Customer? Customer { get; set; }

        public static Order Create(decimal value, int customerId)
        {
            return new Order
            {
                CustomerId = customerId,
                Value = value,
                OrderDate = DateTime.UtcNow
            };
        }
    }
}
