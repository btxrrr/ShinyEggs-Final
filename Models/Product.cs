using Microsoft.AspNetCore.Authorization;
using ShinyEggs.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ShinyEggs.Models
{
    [Table("Product")]
	public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Please enter a valid string.")]

        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Brand is required.")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Please enter a valid string.")]

        public string? Brand { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [RegularExpression(@"^\$?\d+(\.\d{1,2})?$", ErrorMessage = "Invalid price format.")]
		[Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
		[DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
		public double Price { get; set; }

        [ForeignKey("Categories")]
        public int CategoryId { get; set; }

        //[ForeignKey("CategoryId")]
                    
        [MaxLength(500)]

        public string? BriefDescription { get; set; }

        //[Url(ErrorMessage = "Invalid URL format.")]
        [RegularExpression("^[a-zA-Z0-9 ]*$", ErrorMessage = "Please enter a valid string.")]

        public string? Image { get; set; }

		// Add this attribute to ignore the circular reference
		[JsonIgnore]
		public virtual Categories? Categories { get; set; }
		//public List<OrderDetail> OrderDetail { get; set; }
		//public List<CartDetail> CartDetail { get; set; }
	}
}
