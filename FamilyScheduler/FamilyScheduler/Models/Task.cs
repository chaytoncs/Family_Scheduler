using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    public class Task
    {
        [Required]
        [DisplayName("Task ID")]
        public int? TaskID { get; set; } // PK

        [Required]
        [StringLength(100)]
        public string? Description { get; set; }

        [Required]
        [DisplayName("Frequency ID")]
        public int? FrequencyID { get; set; }

        [Required]
        [DisplayName("Task Type ID")]
        public int? TaskTypeID { get; set; } // FK

        [Required]
        [DisplayName("Workload ID")]
        public int? WorkloadID { get; set; } // FK

        // Navigation Properties
        [DisplayName("Task Type")]
        public TaskType? TaskType { get; set; }

        public Workload? Workload { get; set; }

        public Frequency? Frequency { get; set; }

        public ICollection<Assignment>? Assignments { get; set; }
    }
}
