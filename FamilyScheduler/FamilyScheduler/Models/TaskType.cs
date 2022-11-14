using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    public class TaskType
    {
        public int TaskTypeID { get; set; } // PK

        public string Description { get; set; } = string.Empty;

        // Nagivation Properties
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
