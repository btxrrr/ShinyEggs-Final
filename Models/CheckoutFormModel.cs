using System.ComponentModel.DataAnnotations;

namespace ShinyEggs.Models
{
	public class CheckoutFormModel
	{
		[Required(ErrorMessage = "Name is required")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		public string Email { get; set; }
	}
}
