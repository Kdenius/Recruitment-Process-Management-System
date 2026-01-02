using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WebApi.Models;

namespace WebApi.DTO
{
    public class CandidateDTO
    {

        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }
        //education also 
        List<string>? Languages { get; set; }
        public List<int>? CandidateSkillIds { get; set; }

        public string ClientUrl { get; set; }

        public IFormFile resume {  get; set; }
    }
}
