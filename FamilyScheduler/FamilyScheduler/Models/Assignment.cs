using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    // Assignment Model: Used to represent an Assigned Task. Links to a User and a Task via TaskID / UserID.
    public class Assignment
    {
        public int AssignmentID { get; set; } // PK

        public int TaskID { get; set; } // FK

        public int UserID { get; set; } // FK

        [DisplayName("Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [DefaultValue(false)]
        public bool Completed { get; set; }

        // Navigation Properties
        public Task Task { get; set; } = null!;

        public User User { get; set; } = null!;
    }
}
