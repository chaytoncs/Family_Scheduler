using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    public class User
    {
        public int UserID { get; set; } // PK

        public string UserName { get; set; } = string.Empty;

        [DisplayName("First Name")]
        public string FirstName { get; set; } = string.Empty;

        [DisplayName("Last Name")]
        public string LastName { get; set; } = string.Empty;

        // Navigation Properties
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
