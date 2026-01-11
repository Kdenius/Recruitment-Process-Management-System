using Microsoft.AspNetCore.Mvc;
using WebApi.DTO;
using WebApi.Repository;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("application")]
    public class ApplicationController : ControllerBase
    {
        private readonly ICandidateApplicationRepository _candidateApplicationRepository;
        private readonly IEmailService _emailService;

        public ApplicationController(ICandidateApplicationRepository candidateApplicationRepository, IEmailService emailService)
        {
            _candidateApplicationRepository = candidateApplicationRepository;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllApplication()
        {
            var res =  await _candidateApplicationRepository.GetAllCandidateApplicationsAsync();
            return Ok(res);
        }

        [HttpPut("update-status/{applicationId}")]
        public async Task<IActionResult> UpdateStatus(int applicationId, [FromBody] UpdateStatusDTO updateStatusDTO)
        {
            var result = await _candidateApplicationRepository.UpdateApplicationStatusAsync(applicationId, updateStatusDTO.Status, updateStatusDTO.Details);

            var app = await _candidateApplicationRepository.GetApplicationsByIdAsync(applicationId);
            if(!result)
                return NotFound(new { message = "Application not found" });

            try
            {
                if(updateStatusDTO.Status == "Shortlisted")
                    await _emailService.Shortlisted(app.Candidate.Name, app.Candidate.Email, app.Position.Title, DateTime.Now.ToShortDateString());

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>(success: false, message: $"Issue on mail verification, please check Email"));

            }
            return Ok(new { message = "Application status updated successfully" });
        }
    }
}
