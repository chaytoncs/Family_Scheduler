using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    public class Workload
    {
        public int WorkloadID { get; set; } // PK

        public string Description { get; set; } = string.Empty;

        public int Value { get; set; }

        // Navigation Properties
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
