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
                if (updateStatusDTO.Status == "Shortlisted")
                    await _emailService.Shortlisted(app.Candidate.Name, app.Candidate.Email, app.Position.Title, DateTime.Now.ToShortDateString());
                else if (updateStatusDTO.Status == "Selected")
                    await _emailService.SelectedCandidateEmail(app.Candidate.Name, app.Candidate.Email, app.Position.Title);
                else if (updateStatusDTO.Status == "Rejected")
                    await _emailService.RejectedCandidateEmail(app.Candidate.Name, app.Candidate.Email, app.Position.Title);


            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>(success: false, message: $"Issue on mail sending, please check Email : {app.Candidate.Email}"));

            }
            return Ok(new { message = "Application status updated successfully" });
        }
        [HttpGet("applications-with-interviews")]
        public async Task<IActionResult> GetApplicationsWithInterviews()
        {
            var applications = await _candidateApplicationRepository.GetApplicationsWithInterviewsAsync();
            return Ok(applications);
        }

        [HttpGet("applications-with-documents")]
        public async Task<IActionResult> GetApplicationsWithDocuments()
        {
            var applications = await _candidateApplicationRepository
                .GetApplicationsWithDocumentsAsync();

            return Ok(applications);
        }


    }
}
