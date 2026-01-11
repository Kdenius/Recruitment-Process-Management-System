using System.ComponentModel.DataAnnotations;
using WebApi.Models;

namespace WebApi.Models
{
    public class CandidateApplication
    {
        [Key]
        public int ApplicationId { get; set; }

        public int CandidateId { get; set; }
        public Candidate Candidate { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; }

        public string Status { get; set; }
        public string? Details { get; set; }

        public DateTime CreatedAt { get; set; }
        public ICollection<InterviewRound> InterviewRounds { get; set; }
        public ICollection<CandidateDocument> CandidateDocuments { get; set; }
    }
}

