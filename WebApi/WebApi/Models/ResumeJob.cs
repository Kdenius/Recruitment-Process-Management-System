namespace WebApi.Models
{
    public class ResumeJob
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public string FilePath { get; set; }
        public string Status { get; set; } // Pending, Processing, Completed, Failed
        public string? Error { get; set; }
    }
}
