using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    public class Frequency
    {
        public int FrequencyID { get; set; }

        [StringLength(100)]
        public string Description { get; set; } = string.Empty;

        public int Value { get; set; }

        // Navigation Properties
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
