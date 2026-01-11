using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("round-types")]
    public class RoundTypeController : ControllerBase
    {
        private readonly IRoundTypeRepository _repository;

        public RoundTypeController(IRoundTypeRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roundTypes = await _repository.GetAllAsync();
            return Ok(roundTypes);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoundType roundType)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _repository.CreateAsync(roundType);

            return CreatedAtAction(
                nameof(GetAll),
                new { id = created.RoundTypeId },
                created
            );
        }
    }
}
