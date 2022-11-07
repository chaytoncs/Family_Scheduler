using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    public class Task
    {
        [Required]
        [DisplayName("Task ID")]
        public int TaskID { get; set; } // PK

        [Required]
        [StringLength(100)]
        [DisplayName("Description")]
        public string Description { get; set; }

        [Required]
        [DisplayName("Frequency ID")]
        public int FrequencyID { get; set; }

        [Required]
        [DisplayName("Task Type ID")]
        public int TaskTypeID { get; set; } // FK

        [Required]
        [DisplayName("Workload ID")]
        public int WorkloadID { get; set; } // FK

        // Navigation Properties
        [DisplayName("Task Type")]
        public TaskType TaskType { get; set; }

        [DisplayName("Workload")]
        public Workload Workload { get; set; }

        [DisplayName("Frequency")]
        public Frequency Frequency { get; set; }

        [DisplayName("Assignments")]
        public ICollection<Assignment> Assignments { get; set; }
    }
}
