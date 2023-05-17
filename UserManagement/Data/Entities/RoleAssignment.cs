using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class RoleAssignment : BaseEntities
    {
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public Guid RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }
    }
}
