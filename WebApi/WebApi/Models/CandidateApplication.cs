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

        public bool IsOnHold { get; set; }
        public string OnHoldReason { get; set; }

        public ICollection<InterviewRound> InterviewRounds { get; set; }
        public ICollection<CandidateDocument> CandidateDocuments { get; set; }
    }
}

