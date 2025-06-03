using Microsoft.AspNetCore.Identity;

namespace AzurePizza01.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}