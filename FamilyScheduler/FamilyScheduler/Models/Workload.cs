using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    public class Workload
    {
        [Required]
        [DisplayName("Workload ID")]
        public int WorkloadID { get; set; } // PK

        [Required]
        [StringLength(100)]
        [DisplayName("Description")]
        public string Description { get; set; }

        [Required]
        [DisplayName("Value")]
        public int Value { get; set; }

        // Navigation Properties
        [DisplayName("Tasks")]
        public ICollection<Task> Tasks { get; set; }
    }
}
