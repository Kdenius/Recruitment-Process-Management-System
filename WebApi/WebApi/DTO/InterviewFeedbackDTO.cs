namespace WebApi.DTO
{
    public class InterviewRatingDTO
    {
        public int SkillId { get; set; }
        public int Rating { get; set; }
        public string Remark { get; set; }
    }

    public class InterviewFeedbackDTO
    {
        public int InterviewId { get; set; }

        public string FeedbackText { get; set; }
        public int? FeedbackScore { get; set; }

        public List<InterviewRatingDTO>? Ratings { get; set; }
    }
}
