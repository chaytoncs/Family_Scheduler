using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    public class Assignment
    {
        public int AssignmentID { get; set; } // PK

        public int TaskID { get; set; } // FK

        public int UserID { get; set; } // FK

        [DisplayName("Due Date")]
        public DateOnly DueDate { get; set; }

        [DefaultValue(false)]
        public bool Completed { get; set; }

        // Navigation Properties
        public Task Task { get; set; } = null!;

        public User User { get; set; } = null!;
    }
}
