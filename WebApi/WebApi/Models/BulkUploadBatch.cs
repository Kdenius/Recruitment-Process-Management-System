namespace WebApi.Models
{
    public class BulkUploadBatch
    {
        public int Id { get; set; }
        public int TotalFiles { get; set; }
        public int Completed { get; set; }
        public int Failed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
