using WebApi.Enums;

namespace WebApi.DTO
{
    public class InterviewDecisionDTO
    {
        public int RoundId { get; set; }
        public InterviewResult Result { get; set; } // Selected / Rejected

        public string Remark { get; set; }
    }
}
