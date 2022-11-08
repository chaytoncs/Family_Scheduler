using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    public class Frequency
    {
        [Required]
        [DisplayName("Frequency ID")]
        public int? FrequencyID { get; set; }

        [Required]
        [StringLength(100)]
        public string? Description { get; set; }

        [Required]
        public int? Value { get; set; }

        // Navigation Properties
        public ICollection<Task>? Tasks { get; set; }
    }
}
