using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    // Frequency Model: Used to represent an Frequency. Related to a List of Tasks because every Task requires a Frequency.
    public class Frequency
    {
        public int FrequencyID { get; set; }

        public string Description { get; set; } = string.Empty;

        public int Value { get; set; }

        // Navigation Properties
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
