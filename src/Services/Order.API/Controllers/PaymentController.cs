using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Order.API.Data;
using Order.API.Services;
using Stripe;

namespace Order.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly OrderContext _context;
        private readonly StripeSettings _stripeSettings;

        public PaymentsController(
            PaymentService paymentService,
            OrderContext context,
            IOptions<StripeSettings> stripeSettings)
        {
            _paymentService = paymentService;
            _context = context;
            _stripeSettings = stripeSettings.Value;
        }

        // POST: api/payments/{orderId}
        [HttpPost("{orderId:guid}")]
        public async Task<IActionResult> CreatePaymentIntent(Guid orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order is null)
            {
                return NotFound($"Order with ID '{orderId}' not found.");
            }
            if (order.PaymentStatus == "Succeeded")
            {
                return BadRequest("Payment has already been completed for this order.");
            }
            var intent = await _paymentService.CreatePaymentIntentAsync(order.Total, order.Id, "eur");

            return Ok(new
            {
                clientSecret = intent.ClientSecret,
                publicKey = _stripeSettings.PublicKey
            });
        }

        // POST: api/payments/webhook
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeSignature = Request.Headers["Stripe-Signature"].ToString();
                if (string.IsNullOrEmpty(stripeSignature))
                {
                    return BadRequest("Missing header Stripe-Signature.");
                }

                var stripeEvent = _paymentService.ConstructWebhookEvent(
                    json,
                    stripeSignature,
                    _stripeSettings.WebhookSecret
                );

                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                var orderIdString = paymentIntent?.Metadata["orderId"];

                if (string.IsNullOrEmpty(orderIdString))
                {
                    return BadRequest("Missing Order Id in Payment Intent metadata.");
                }

                if (Guid.TryParse(orderIdString, out var orderId))
                {
                    var order = await _context.Orders.FindAsync(orderId);
                    if (order != null)
                    {
                        if (stripeEvent.Type == "payment_intent.succeeded")
                        {
                            order.PaymentStatus = "Succeeded";
                        }
                        else if (stripeEvent.Type == "payment_intent.payment_failed")
                        {
                            order.PaymentStatus = "Failed";
                        }
                        await _context.SaveChangesAsync();
                    }
                }
          
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
