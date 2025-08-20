using ProvaPub.Enums;

namespace ProvaPub.Services.Payments
{
    public class PixPaymentService: BasePaymentService
    {
        public override PaymentMethod Method => PaymentMethod.Pix;
    }
}
