using ProvaPub.Enums;

namespace ProvaPub.Services.Payments
{
    public class CreditCardPaymentService : BasePaymentService
    {
        public override PaymentMethod Method => PaymentMethod.CreditCard;
    }
}
