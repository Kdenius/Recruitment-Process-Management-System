using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Controllers
{
    [Route("skill")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly ISkillRepository _skillRepository;

        public SkillController(ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSkill([FromBody] string skill)
        {;

            var createdSkill = await _skillRepository.CreateSkillAsync(skill);

            return CreatedAtAction(nameof(GetSkillById), new { id = createdSkill.SkillId }, createdSkill);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            var skills = await _skillRepository.GetAllSkillsAsync();
            return Ok(skills);
        }

        // GET: api/skill/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSkillById(int id)
        {
            var skill = await _skillRepository.GetSkillByIdAsync(id);

            if (skill == null)
                return NotFound();

            return Ok(skill);
        }
    }
}
