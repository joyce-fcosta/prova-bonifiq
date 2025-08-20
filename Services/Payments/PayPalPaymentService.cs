using ProvaPub.Enums;

namespace ProvaPub.Services.Payments
{
    public class PayPalPaymentService : BasePaymentService
    {
        public override PaymentMethod Method => PaymentMethod.PayPal;
    }
}
