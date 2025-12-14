namespace WebApi.DTO
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? data { get; set; }
        public ApiResponse(bool success, string message, T? data)
        {
            Success = success;
            Message = message;
            this.data = data;
        }
    }
}
