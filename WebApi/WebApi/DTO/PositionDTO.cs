using System.ComponentModel.DataAnnotations;
using WebApi.Models;

namespace WebApi.DTO
{
    public class PositionDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }
        public string Status { get; set; } // Open,On Hold, Closed
        public int Rounds { get; set; }
        public string Type { get; set; }
        public int BaseSalary { get; set; }
        public int MaxSalary { get; set; }
        public string Location { get; set; }    

        public int RecruiterId { get; set; }

        public List<int> SkillIds { get; set; } // List of SkillIds for the Position
    }
}
