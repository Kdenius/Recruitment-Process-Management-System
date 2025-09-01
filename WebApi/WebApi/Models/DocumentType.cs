using System.ComponentModel.DataAnnotations;
using WebApi.Models;

namespace WebApi.Models
{
    public class DocumentType
    {
        [Key]
        public int DocTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public string DocTypeName { get; set; }
    }
}