using Job1670.Data;
using Microsoft.AspNetCore.Builder;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Job1670.Models
{
    [Table("Employer")]
    public class Employer
    {
        [Key]
        public string EmployerId { get; set; }

        [Required]
        public string CompanyName { get; set; }
        public string Address { get; set; }

        public string? Detail { get; set; }
        // Danh sách các job applications đã duyệt
        public virtual List<JobApplication> ApprovedJobApplications { get; set; } = new List<JobApplication>();


        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [ForeignKey("ApplicationUserId")]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; } // Liên kết với AspNetUser
    }
}
