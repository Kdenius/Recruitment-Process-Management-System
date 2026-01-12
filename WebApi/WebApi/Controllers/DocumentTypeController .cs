using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("document-types")]
    public class DocumentTypeController : ControllerBase
    {
        private readonly IDocumentTypeRepository _repository;

        public DocumentTypeController(IDocumentTypeRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _repository.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DocumentType documentType)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _repository.CreateAsync(documentType);
            return CreatedAtAction(nameof(GetAll), new { id = created.DocTypeId }, created);
        }
    }
}
