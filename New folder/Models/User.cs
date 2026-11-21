using Microsoft.AspNetCore.Identity;

namespace finalProject.Models
{
    public class User : IdentityUser<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User";
    }
}
