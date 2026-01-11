using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    [Table("RoundType")]
    public class RoundType
    {
        [Key]
        public int RoundTypeId { get; set; }
        [Required]
        [StringLength(30)]
        public string TypeName { get; set; }
    }
}
