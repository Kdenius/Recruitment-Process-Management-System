namespace WebApi.DTO
{

    public class ApplicationDocumentDTO
    {
        public int DocumentId { get; set; }
        public int DocTypeId { get; set; }
        public string DocTypeName { get; set; }
        public string? DocumentUrl { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? UploadedAt { get; set; }
    }
    public class ApplicationWithDocumentsDTO
    {
        public int ApplicationId { get; set; }
        public string CandidateName { get; set; }
        public string Position { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<ApplicationDocumentDTO> Documents { get; set; } = new();
    }
}
