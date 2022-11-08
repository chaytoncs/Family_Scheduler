using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    public class Workload
    {
        [Required]
        [DisplayName("Workload ID")]
        public int? WorkloadID { get; set; } // PK

        [Required]
        [StringLength(100)]
        public string? Description { get; set; }

        [Required]
        public int? Value { get; set; }

        // Navigation Properties
        public ICollection<Task>? Tasks { get; set; }
    }
}
