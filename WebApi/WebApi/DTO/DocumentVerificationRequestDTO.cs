namespace WebApi.DTO
{
    public class DocumentVerificationRequestDTO
    {
        public int ApplicationId { get; set; }
        public List<int> RequiredDocTypeIds { get; set; }
    }
}
