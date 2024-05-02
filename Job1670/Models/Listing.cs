using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
    }
}
