using Data.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class RoleAssignmentModel : BaseModel
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }

    public class RoleAssignmentViewModel : BaseModel
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
    public class RoleAssignmentCreateModel
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }

    public class RoleAssignmentUpdateModel : BaseModel
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        [JsonIgnore]
        public DateTime DateUpdate { get; set; } = DateTime.UtcNow;
    }

    public class UserRole
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? RoleName { get; set; }
    }
}
