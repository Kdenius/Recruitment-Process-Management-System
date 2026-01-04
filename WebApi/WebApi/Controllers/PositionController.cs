using Azure.Core.GeoJson;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTO;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Controllers
{
    [Route("/position")]
    [ApiController]
    public class PositionController : ControllerBase
    {
        private readonly IPositionRepository _positionRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly IUserRepository _userRepository;

        public PositionController(IPositionRepository positionRepository, ISkillRepository skillRepository, IUserRepository userRepository)
        {
            _positionRepository = positionRepository;
            _skillRepository = skillRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePosition([FromBody] PositionDTO positionDTO)
        {
            if (positionDTO == null)
                return BadRequest("Position data is invalid.");
            var skills = await _skillRepository.GetSkillsByIdsAsync(positionDTO.SkillIds);

            var position = new Position()
            {
                Title = positionDTO.Title,
                Description = positionDTO.Description,
                Status = positionDTO.Status,
                Rounds = positionDTO.Rounds,
                Type = positionDTO.Type,
                BaseSalary = positionDTO.BaseSalary,
                MaxSalary = positionDTO.MaxSalary,
                Location = positionDTO.Location,
                CreatedAt = DateTime.UtcNow,
                Recruiter = _userRepository.GetById(positionDTO.RecruiterId),
                PositionSkills = skills.Select(s => new PositionSkill
                {
                    SkillId = s.SkillId,
                    Skill = s,
                    IsRequired = true // Assuming all skills attached are required
                }).ToList()
            };

            var createdPosition = await _positionRepository.CreatePositionAsync(position);

            return CreatedAtAction(nameof(GetPosition), new { id = createdPosition.PositionId }, createdPosition);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPositions()
        {
            var pos = await _positionRepository.GetAllPositionsAsync();
            return Ok(pos);
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPosition(int id)
        {
            var position = await _positionRepository.GetPositionByIdAsync(id);

            if (position == null)
                return NotFound();

            return Ok(position);
        }

    }
}
