using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class PermissionAssignment : BaseEntities
    {
        public Guid RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role? Roles { get; set; }
        public Guid PermissionId { get; set; }
        [ForeignKey("PermissionId")]
        public Permission? Permissions { get; set; }
    }
}
