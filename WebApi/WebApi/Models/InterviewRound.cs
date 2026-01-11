using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WebApi.Enums;

namespace WebApi.Models
{
    public class InterviewRound
    {
        [Key]
        public int RoundId { get; set; }

        [ForeignKey("CandidateApplication")]
        public int ApplicationId { get; set; }
        public CandidateApplication CandidateApplication { get; set; }

        public int RoundNumber { get; set; }

        public InterviewMode Mode { get; set; }
        public DateTime? ScheduledAt { get; set; }


        public string? Location { get; set; }  
        public string? MeetingLink { get; set; }

        public bool IsCompleted { get; set; }

        public InterviewResult Result { get; set; } = InterviewResult.Draft;
        public int RoundTypeId { get; set; }
        public RoundType RoundType { get; set; }

        [JsonIgnore]
        public ICollection<Interview> Interviews { get; set; }
    }
}
