using Job1670.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Job1670.Models
{
    [Table("JobSeeker")]
    public class JobSeeker
    {
        [Key]
        public string JobSeekerId { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string? Detail { get; set; }
        public string Email { get; set; }

        public string? CV { get; set; }

        [ForeignKey("ApplicationUserId")]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; } // Liên kết với AspNetUser
    }
}
