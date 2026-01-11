using WebApi.Enums;

namespace WebApi.DTO
{
    public class ScheduleInterviewDTO
    {
        public int ApplicationId { get; set; }
        public int RoundTypeId { get; set; }
        public int RoundNumber { get; set; }

        public DateTime ScheduledAt { get; set; }
        public InterviewMode Mode { get; set; }

        public string? Location { get; set; }
        public string? MeetingLink { get; set; }

        public List<int> InterviewerIds { get; set; }
    }
}
