namespace WebApi.DTO
{
    public class UploadDocumentDTO
    {
        public int ApplicationId { get; set; }
        public int DocTypeId { get; set; }
        public IFormFile File { get; set; }
    }

}
