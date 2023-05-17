using Data.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class RoleModel
    {
    }

    public class RoleViewModel : BaseModel
    {
        public string? Name { get; set; }
    }
    public class RoleCreateModel
    {
        [Required]
        public string? Name { get; set; }
    }

    public class RoleUpdateModel : BaseModel
    {
        [Required]
        public string? Name { get; set; }
        [JsonIgnore]
        public DateTime DateUpdate { get; set; } = DateTime.UtcNow;
    }

    public class RoleQueryModel : QueryStringParameters
    {
        public RoleQueryModel()
        {
            OrderBy = "Name";
            OrderBy = "DateCreate";
        }
        public string? Search { get; set; }
    }
}
