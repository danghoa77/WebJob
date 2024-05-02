using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Job1670.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();
    }
}
