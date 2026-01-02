namespace WebApi.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
    public class FileUploadService : IFileUploadService
    {
        private readonly string _uploadFolderPath;

        public FileUploadService()
        {
            _uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(_uploadFolderPath))
            {
                Directory.CreateDirectory(_uploadFolderPath);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {   

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(_uploadFolderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return filePath;
        }
    }
}
