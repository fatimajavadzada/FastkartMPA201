using Microsoft.AspNetCore.Identity;

namespace FastkartMPA201.Models;

public class AppUser : IdentityUser
{
    public string FullName { get; set; }
}
