using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTO;
using WebApi.Models;
using WebApi.Repository;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("candidate")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateRepository _candidateRepository;
        private readonly IEmailService _emailService;
        private readonly IRoleRepository _roleRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly ISkillRepository _skillRepository;
        private readonly ICandidateApplicationRepository _candidateApplicationRepository;
        private readonly IPositionRepository _positionRepository;

        public CandidateController(ICandidateRepository candidateRepository, IEmailService emailService, IRoleRepository roleRepository, IFileUploadService fileUploadService, ISkillRepository skillRepository, ICandidateApplicationRepository candidateApplicationRepository, IPositionRepository positionRepository)
        {
            _candidateRepository = candidateRepository;
            _emailService = emailService;
            _roleRepository = roleRepository;
            _fileUploadService = fileUploadService;
            _skillRepository = skillRepository;
            _candidateApplicationRepository = candidateApplicationRepository;
            _positionRepository = positionRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCandidateAsync([FromForm] CandidateDTO candidateDTO)
        {
            
            if (candidateDTO == null)
            {
                return BadRequest("Candidate data is missing.");
            }

            if (candidateDTO.resume == null || candidateDTO.resume.Length == 0)
            {
                return BadRequest("No resume file uploaded.");
            }

          
           var skills = await _skillRepository.GetSkillsByIdsAsync(candidateDTO.CandidateSkillIds);
            var resumeFilePath = await _fileUploadService.UploadFileAsync(candidateDTO.resume);
            var candidate = new Candidate
            {
                Name = candidateDTO.Name,
                Email = candidateDTO.Email,
                PhoneNumber = candidateDTO.PhoneNumber,
                PasswordHash = TokenGenerator.GenerateRandomizeToken(),
                CurrentStatus = "Active",
                ResumeUrl = resumeFilePath.ToString(),
                CreatedAt = DateTime.Now,
                CandidateSkills = skills.Select(s => new CandidateSkill
                {
                    SkillId = s.SkillId,
                    Skill = s,
                }).ToList()
            };
            var createdCandidate = await _candidateRepository.CreateCandidateAsync(candidate);

            try
            {
                await _emailService.ActivationEmail(createdCandidate, candidateDTO.ClientUrl);

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>(success: false, message: $"Issue on mail verification, please check Address: {candidateDTO.Email}"));

            }
            return Ok(candidate);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCandidate()
        {
            var pos = await _candidateRepository.GetAllCandidatesAsync();
            return Ok(pos);
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyForJobAsync([FromBody] JobApplicationDTO jobApplicationDTO)
        {
            if (jobApplicationDTO == null || jobApplicationDTO.CandidateId <= 0 || jobApplicationDTO.PositionId <= 0)
            {
                return BadRequest("Invalid application data.");
            }

            var candidateApplication = new CandidateApplication
            {
                CandidateId = jobApplicationDTO.CandidateId,
                PositionId = jobApplicationDTO.PositionId,
                IsOnHold = false,
                OnHoldReason = string.Empty
            };

            await _candidateApplicationRepository.CreateApplication(candidateApplication);
            var can = await _candidateRepository.GetById(candidateApplication.CandidateId);
            var job = await _positionRepository.GetPositionByIdAsync(candidateApplication.PositionId);
            try
            {
                await _emailService.ApplicationAknow(can.Name, can.Email, job.Title, DateTime.Now.ToShortDateString());

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>(success: false, message: $"error on mail, please check Address"));

            }

            return Ok(candidateApplication);
        }
    }
}
