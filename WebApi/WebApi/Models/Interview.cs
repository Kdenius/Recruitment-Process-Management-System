using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class Interview
    {
        [Key]
        public int InterviewId { get; set; }

        [ForeignKey("InterviewRound")]
        public int RoundId { get; set; }
        public InterviewRound InterviewRound { get; set; }

        public int InterviewerId { get; set; }
        public User Interviewer { get; set; }

        public string? FeedbackText { get; set; }

        public int? FeedbackScore { get; set; }//1 thi 10

        public DateTime? CompletedAt { get; set; }

        public ICollection<InterviewRating> InterviewRatings { get; set; }
    }
}
