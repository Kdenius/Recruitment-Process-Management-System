namespace WebApi.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file);
        void DeleteFile(string filePath);
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
            var fileName = $"{Guid.NewGuid()}{(file.FileName)}";
            var physicalPath = Path.Combine(_uploadFolderPath, fileName);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"uploads/{fileName}";
        }
        public async void DeleteFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            var fileName = Path.GetFileName(filePath);
            var physicalPath = Path.Combine(_uploadFolderPath, fileName);

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }
    }
}
