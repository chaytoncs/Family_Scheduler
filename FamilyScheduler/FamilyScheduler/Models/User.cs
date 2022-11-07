using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FamilyScheduler.Models
{
    public class User
    {
        [Required]
        [DisplayName("User ID")]
        public int UserID { get; set; } // PK

        [Required]
        [StringLength(75)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(75)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        // Navigation Properties
        [DisplayName("Assignments")]
        public ICollection<Assignment> Assignments { get; set; }
    }
}
