using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace WebApi.Models
{
    public class OfferLetter
    {
        [Key]
        public int OfferId { get; set; }

        public int ApplicationId { get; set; }
        public CandidateApplication CandidateApplication { get; set; }

        public int  SalaryOffered { get; set; }
        public string OfferUrl { get; set; }

        public int SentByUserId { get; set; }
        public User SentByUser { get; set; }

        public DateTime SentAt { get; set; }
    }
}
