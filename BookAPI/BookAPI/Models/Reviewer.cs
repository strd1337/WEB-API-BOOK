using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookAPI.Models
{
    public class Reviewer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(80, ErrorMessage = "First name cannot be more than 80 characters")]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Last name cannot be more than 100 characters")]
        public string? LastName { get; set; }

        public virtual ICollection<Review>? Reviews { get; set; }
    }
}
