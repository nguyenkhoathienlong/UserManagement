

using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class Role : BaseEntities
    {
        [Required]
        public string? Name { get; set; }

        // Relationship
        public IList<PermissionAssignment>? PermissionAssignment { get; set; }
    }
}
