using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    public class TaskType
    {
        [Required]
        [DisplayName("Task Type ID")]
        public int? TaskTypeID { get; set; } // PK

        [Required]
        [StringLength(100)]
        public string? Description { get; set; }

        // Nagivation Properties
        public ICollection<Task>? Tasks { get; set; }
    }
}
