using System.ComponentModel;

namespace FamilyScheduler.Models.DTO
{
    // Data Transfer Object for Tasks. Used when I need to send Task related data to the view.
    public class TaskDTO
    {
        public int TaskID { get; set; } // PK

        public string Description { get; set; } = string.Empty;

        public int FrequencyID { get; set; } // FK

        public int TaskTypeID { get; set; } // FK

        public int WorkloadID { get; set; } // FK

        [DisplayName("Workload")]
        public string? WorkloadDescription { get; set; }

        [DisplayName("Workload Value")]
        public int WorkloadValue { get; set; }

        [DisplayName("Frequency")]
        public string? FrequencyDescription { get; set; }

        [DisplayName("Frequency Value")]
        public int FrequencyValue { get; set; }

        [DisplayName("Task Type")]
        public string? TaskTypeDescription { get; set; }
    }
}
