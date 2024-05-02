using Job1670.Models;
using Microsoft.AspNetCore.Identity;

namespace Job1670.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? ProfilePicture { get; set; }
        public virtual Employer Employer { get; set; } // Mối quan hệ "Một - Một" với Employer
        public virtual JobSeeker JobSeeker { get; set; } // Mối quan hệ "Một - Một" với JobSeeker
    }
}
