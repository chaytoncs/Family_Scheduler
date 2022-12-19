using System.ComponentModel;

namespace FamilyScheduler.Models
{
    public class Schedule
    {
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }

        [DisplayName("Max Assignments")]
        public int MaxAssignments { get; set; }
    }
}
