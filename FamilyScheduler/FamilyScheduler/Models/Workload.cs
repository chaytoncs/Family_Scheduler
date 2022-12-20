using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    // Workload Model: Used to represent an Workload. Related to a List of Tasks because every Task requires a Workload.
    public class Workload
    {
        public int WorkloadID { get; set; } // PK

        public string Description { get; set; } = string.Empty;

        public int Value { get; set; }

        // Navigation Properties
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
