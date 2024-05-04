using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Job1670.Data;

namespace Job1670.Models
{
    [Table("Listing")]
    public class Listing
    {
        [Key]
        public int JobId { get; set; }

        [Required]
        public string EmployerId { get; set; }

        [ForeignKey("EmployerId")]
        public virtual Employer Employer { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public string Title { get; set; }

        public DateTime Deadline { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();

        public List<JobSeeker> GetAppliedJobSeekers(ApplicationDbContext context)
        {
            // Truy vấn các jobseeker dựa trên Id của listing và employer
            var appliedJobSeekers = context.JobApplications
                .Where(j => j.JobId == this.JobId && j.Listing.EmployerId == this.EmployerId)
                .Select(j => j.JobSeeker)
                .Distinct() // Loại bỏ các jobseeker trùng lặp
                .ToList();

            return appliedJobSeekers;
        }
    }
}
