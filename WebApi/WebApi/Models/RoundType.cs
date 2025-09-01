using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class RoundType
    {
        [Key]
        public int RoundTypeId { get; set; }
        [Required]
        [StringLength(30)]
        public string TypeName { get; set; }
        public ICollection<CandidateApplication> InterviewRounds { get; set; }
    }
}
