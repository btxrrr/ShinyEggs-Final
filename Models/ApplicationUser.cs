using Microsoft.AspNetCore.Identity;
namespace ShinyEggs.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
    }
}