using Data.Entities;
using Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Data.Models
{
    public class UserModel : BaseModel
    {

    }

    public class UserViewModel : BaseModel
    {
        public string? UserName { get; set; }
        [JsonIgnore]
        public string? Password { get; set; }
        public string? FullName { get; set; } = string.Empty;
        public Gender? Gender { get; set; }
        public string? Phone { get; set; }
    }

    public class UserCreateModel
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? FullName { get; set; } = string.Empty;
        [Required]
        public Gender? Gender { get; set; }
        [Required]
        public string? Phone { get; set; }
    }

    public class UserUpdateModel : BaseModel
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; } = string.Empty;
        public Gender? Gender { get; set; }
        public string? Phone { get; set; }
        [JsonIgnore]
        public DateTime DateUpdate { get; set; } = DateTime.UtcNow;
    }

    public class UserQueryModel : QueryStringParameters
    {
        public UserQueryModel()
        {
            OrderBy = "FullName";
            OrderBy = "DateCreate";
        }
        public string? Search { get; set; }
    }

    public class UserRequest
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

    }

    public class UserRegister : BaseModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        [Required]
        public string? FullName { get; set; } = string.Empty;
        [Required]
        public Gender? Gender { get; set; }
        [Required]
        [Phone(ErrorMessage = "Phone must be formated")]
        public string? Phone { get; set; }
    }
}
