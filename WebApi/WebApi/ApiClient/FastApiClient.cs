using WebApi.DTO;

namespace WebApi.ApiClient
{
    public class FastApiClient
    {
        private readonly HttpClient _client;

        public FastApiClient(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("http://localhost:8000");
        }
        public async Task<ParseResumeDTO> ParseResume(string filePath)
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StreamContent(File.OpenRead(filePath)), "file", Path.GetFileName(filePath));

            var response = await _client.PostAsync("/parse-resume", form);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ParseResumeDTO>();
        }
    }
}
