using Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Data.Entities
{
    public class User : BaseEntities
    {
        [Required]
        [StringLength(50, ErrorMessage = "User Name can't be longer than 50 characters")]
        public string? UserName { get; set; }
        [Required]
        [JsonIgnore]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name can't be longer than 50 characters")]
        public string? FullName { get; set; }
        public Gender? Gender { get; set; }
        [Phone(ErrorMessage = "Phone is not true to the format")]
        [StringLength(13, ErrorMessage = "Phone number up to 13 characters long")]
        public string? Phone { get; set; }

        // Relationship
        public IList<RoleAssignment>? RoleAssignments { get; set; }

    }
}
