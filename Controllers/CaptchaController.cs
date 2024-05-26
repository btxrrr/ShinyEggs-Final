using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ShinyEggs.Models;

namespace ShinyEggs.Controllers
{
	public class CaptchaController : Controller
	{
		private readonly ReCaptchaService _reCaptchaService;

		public CaptchaController(ReCaptchaService reCaptchaService)
		{
			_reCaptchaService = reCaptchaService;
		}

		[HttpPost]
		public async Task<ActionResult> FormSubmit()
		{
			var recaptchaResponse = HttpContext.Request.Form["g-recaptcha-response"];

			// Validate the recaptchaResponse input
			if (string.IsNullOrEmpty(recaptchaResponse))
			{
				// Handle invalid reCAPTCHA response here
				ViewData["Message"] = "Please complete the reCAPTCHA validation.";
				return View("Index");
			}

			var isValidCaptcha = await _reCaptchaService.VerifyRecaptchaAsync(recaptchaResponse);

			if (!isValidCaptcha)
			{
				// Handle invalid reCAPTCHA here, e.g., show an error message to the user
				ViewData["Message"] = "reCAPTCHA validation failed";
				return View("Index");
			}

			ViewData["Message"] = "reCAPTCHA validation success";

			// Continue processing the form submission
			// ...

			return View("Index");
		}
	}
}
