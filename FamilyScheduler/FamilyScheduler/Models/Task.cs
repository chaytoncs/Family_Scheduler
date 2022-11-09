using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    public class Task
    {
        public int TaskID { get; set; } // PK

        [StringLength(100)]
        public string Description { get; set; } = string.Empty;

        public int FrequencyID { get; set; } // FK

        public int TaskTypeID { get; set; } // FK

        public int WorkloadID { get; set; } // FK

        // Navigation Properties
        [DisplayName("Task Type")]
        public TaskType TaskType { get; set; } = null!;

        public Workload Workload { get; set; } = null!;

        public Frequency Frequency { get; set; } = null!;

        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
