using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    public class Assignment
    {
        [Required]
        [DisplayName("Assignment ID")]
        public int AssignmentID { get; set; } // PK

        [Required]
        [DisplayName("Task ID")]
        public int TaskID { get; set; } // FK

        [Required]
        [DisplayName("User ID")]
        public int UserID { get; set; } // FK

        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Due Date")]
        public DateTime DueDate { get; set; }

        [Required]
        [DefaultValue(false)]
        [DisplayName("Completed")]
        public bool Completed { get; set; }

        // Navigation Properties
        [DisplayName("Task")]
        public Task Task { get; set; }

        [DisplayName("User")]
        public User User { get; set; }
    }
}
