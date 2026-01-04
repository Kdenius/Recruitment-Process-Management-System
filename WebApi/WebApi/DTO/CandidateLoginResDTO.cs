using WebApi.Models;

namespace WebApi.DTO
{
    public class CandidateLoginResDTO
    {
        public int CandidateId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleName { get; set; }

        public string CurrentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ? ResumeUrl { get; set; }
        public List<object> candidateSkills { get; set; }
    }
}
