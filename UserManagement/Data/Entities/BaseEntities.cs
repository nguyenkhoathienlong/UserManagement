using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class BaseEntities
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public DateTime DateCreate { get; set; } = DateTime.UtcNow;
        public DateTime DateUpdate { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
    }
}
