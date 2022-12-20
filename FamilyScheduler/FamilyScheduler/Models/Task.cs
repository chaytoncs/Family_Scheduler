using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    // Task Model: Used to represent a Task. Links to a Frequency, Workload, and TaskType. Tasks also link to a List of Assignments because
    // every Assignment requires a Task.
    public class Task
    {
        public int TaskID { get; set; } // PK

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
