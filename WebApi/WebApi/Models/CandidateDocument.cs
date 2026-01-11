using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class CandidateDocument
    {
        [Key]
        public int DocumentId { get; set; }

        [ForeignKey("CandidateApplication")]
        public int ApplicationId { get; set; }
        public CandidateApplication CandidateApplication { get; set; }

        [ForeignKey("DocumentType")]
        public int DocTypeId { get; set; }
        public DocumentType DocumentType { get; set; }

        public string DocumentUrl { get; set; }
        public bool IsVerified { get; set; }

        public DateTime UploadedAt { get; set; }
    }
}
