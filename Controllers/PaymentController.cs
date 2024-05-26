using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.IO;
using System.Threading.Tasks;
using ShinyEggs.Models;

namespace ShinyEggs.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ICartRepository _cartRepo;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        private const string StripeSecretKeyConfigKey = "stripe:SecretKey";
        private const string StripePublishableKeyConfigKey = "stripe:PublishableKey";
        private const string StripeWebhookSigningSecret = "whsec_Nen0TXWQn5GtEsLUMWF9RhDwFImQJzOT";

        public PaymentController(
            ICartRepository cartRepo,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            _cartRepo = cartRepo;
            _configuration = configuration;
            _userManager = userManager;
        }

        private string GetUserId()
        {
            var userId = _userManager.GetUserId(User);
            return userId;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentIntent()
        {
            StripeConfiguration.ApiKey = _configuration.GetSection(StripeSecretKeyConfigKey).Value;

            var userId = GetUserId();
            var cartTotalAmount = await _cartRepo.GetCartTotalAmount(userId);

            var options = new PaymentIntentCreateOptions
            {
                Amount = Convert.ToInt64(cartTotalAmount * 100),
                Currency = "sgd",
                // Set other options as needed
            };

            var service = new PaymentIntentService();
            var intent = service.Create(options);

            var viewModel = new StripeClientSecretViewModel
            {
                ClientSecret = intent.ClientSecret,
                StripePublishableKey = _configuration.GetSection(StripePublishableKeyConfigKey).Value
            };

            return View("Checkout", viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult HandleStripeWebhook()
        {
            var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], StripeWebhookSigningSecret);

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                // Update order status or perform other actions
                // For example, mark the order as paid or send order confirmation emails

                LogPaymentSuccess(paymentIntent);

                return Ok();
            }

            return Ok();
        }

        private void LogPaymentSuccess(PaymentIntent paymentIntent)
        {
            // Customize this method to log payment success events
            // For example, write to a log file or send notifications
            // Here, we're printing to the console for demonstration purposes
            Console.WriteLine($"Payment succeeded for PaymentIntent ID: {paymentIntent.Id}");
        }

		public IActionResult Checkout()
		{
			var viewModel = new CheckoutFormModel();
			return View(viewModel);
		}

	}
}
