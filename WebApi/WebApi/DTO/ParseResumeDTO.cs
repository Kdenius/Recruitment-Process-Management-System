using System.Text.Json.Serialization;

namespace WebApi.DTO
{
    public class EducationDto
    {
        [JsonPropertyName("university_name")]
        public string UniversityName { get; set; }
        public string Degree { get; set; }
        public double? Gpa { get; set; }
    }

    public class ExperienceDto
    {
        [JsonPropertyName("company_name")]
        public string CompanyName { get; set; }
        [JsonPropertyName("n_years")]
        public int? NYears { get; set; }
        [JsonPropertyName("project_name")]
        public string ProjectName { get; set; }
        [JsonPropertyName("project_description")]
        public string ProjectDescription { get; set; }
        [JsonPropertyName("tech_stack")]
        public string TechStack { get; set; }
    }

    public class ParseResumeDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        public List<ExperienceDto> Experience { get; set; }
        public List<EducationDto> Education { get; set; }
        public List<string> Skills { get; set; }
    }
}
