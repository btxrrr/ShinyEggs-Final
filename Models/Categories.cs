using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static System.Web.Razor.Parser.SyntaxConstants;
using Newtonsoft.Json;

namespace ShinyEggs.Models
{
    [Table("Categories")]
    public class Categories
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string CategoryName { get; set; }

		// Add this attribute to ignore the circular reference
		[JsonIgnore]
		public virtual ICollection<Product> Products { get; set; }
	}
}
