using System.ComponentModel;

namespace FamilyScheduler.Models
{
    // Schedule Model: Used to represent an Schedule. Schedule data is not saved to the database. The Schedule Model mainly exists to provide validation for inputs when
    // generating a new schedule.
    public class Schedule
    {
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }

        [DisplayName("Max Assignments")]
        public int MaxAssignments { get; set; }
    }
}
