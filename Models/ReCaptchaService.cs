using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShinyEggs.Models
{
	public class ReCaptchaService
	{
		private const string RecaptchaSecretKey = "6Le1A4onAAAAAL2GfsMH9ePN4WvxRgd8iHaeh7xR";

		public async Task<bool> VerifyRecaptchaAsync(string response)
		{
			using var client = new HttpClient();
			var uri = $"https://www.google.com/recaptcha/api/siteverify?secret={RecaptchaSecretKey}&response={response}";
			var result = await client.GetStringAsync(uri);
			var captchaResponse = JsonConvert.DeserializeObject<RecaptchaResponse>(result);
			return captchaResponse.Success;
		}
	}

	public class RecaptchaResponse
	{
		public bool Success { get; set; }
		// Other properties that the reCAPTCHA API response may include
	}
}

