namespace WebApi.DTO
{
    public class ApplicationWithInterviewsDTO
    {
        public int ApplicationId { get; set; }
        public string CandidateName { get; set; }
        public string Position { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<ApplicationInterviewDTO> Interviews { get; set; }
    }

    public class ApplicationInterviewDTO
    {
        public int InterviewId { get; set; }
        public int RoundNumber { get; set; }
        public string RoundType { get; set; }
        public bool IsCompleted { get; set; }
        public string Result { get; set; }

        public InterviewDetailsDTO? Details { get; set; }
    }

    public class InterviewDetailsDTO
    {
        public int FeedbackScore { get; set; }
        public string FeedbackText { get; set; }
        public List<InterviewSkillRatingDTO> Ratings { get; set; }
    }

    public class InterviewSkillRatingDTO
    {
        public string Skill { get; set; }
        public int Rating { get; set; }
        public string Remark { get; set; }
    }

}
