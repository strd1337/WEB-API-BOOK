using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookAPI.Models
{
    public class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Country name cannot be more than 50 characters")]
        public string? Name { get; set; }

        public virtual ICollection<Author>? Authors { get; set; }
    }
}
