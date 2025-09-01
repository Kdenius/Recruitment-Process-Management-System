using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class InterviewRound
    {
        [Key]
        public int RoundId { get; set; }

        public int ApplicationId { get; set; }
        public CandidateApplication CandidateApplication { get; set; }

        public int RoundNumber { get; set; }


        public DateTime? ScheduledAt { get; set; }

        public string Location { get; set; }

        public bool IsCompleted { get; set; }

        public int RoundTypeId { get; set; }
        public RoundType RoundType { get; set; }

        public ICollection<Interview> Interviews { get; set; }
    }
}
