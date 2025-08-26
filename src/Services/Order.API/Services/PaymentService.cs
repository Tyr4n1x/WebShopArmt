using Microsoft.Extensions.Options;
using Stripe;

namespace Order.API.Services
{
    public class PaymentService
    {
        private readonly string _secretKey;

        public PaymentService(IOptions<StripeSettings> stripeSettings)
        {
            _secretKey = stripeSettings.Value.SecretKey;
            StripeConfiguration.ApiKey = _secretKey;
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency = "eur")
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Stripe works in cents
                Currency = currency,
                PaymentMethodTypes = ["bancontact", "card", "ideal", "paypal", "sepa_debit", "sofort"]
            };

            var service = new PaymentIntentService();
            return await service.CreateAsync(options);
        }

        public Event ConstructWebhookEvent(string json, string stripeSignature, string webhookSecret)
        {
            return EventUtility.ConstructEvent(json, stripeSignature, webhookSecret);
        }
    }
}
