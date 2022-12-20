using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FamilyScheduler.Models.DTO
{
    // Data Transfer Object for Assignments. Used when I need to send Assignment related data to the view.
    public class AssignmentDTO
    {
        public int AssignmentID { get; set; } // PK

        public int TaskID { get; set; } // FK

        public int UserID { get; set; } // FK

        [DisplayName("Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [DefaultValue(false)]
        public bool Completed { get; set; }

        [DisplayName("Task")]
        public string? TaskDescription { get; set; }

        [DisplayName("Workload")]
        public string? WorkloadDescription { get; set; }

        [DisplayName("Frequency")]
        public string? FrequencyDescription { get; set; }

        [DisplayName("Task Type")]
        public string? TaskTypeDescription { get; set; }

        [DisplayName("Name")]
        public string? FullName { get; set; }
    }
}
