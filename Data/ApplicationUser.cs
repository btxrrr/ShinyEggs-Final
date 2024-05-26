using Microsoft.AspNetCore.Identity;
namespace ShinyEggs.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
    }
}
