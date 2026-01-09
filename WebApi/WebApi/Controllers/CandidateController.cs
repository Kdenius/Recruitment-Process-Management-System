using Hangfire;
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
        private readonly IBatchRepository _batchRepository;

        public CandidateController(ICandidateRepository candidateRepository, IEmailService emailService, IRoleRepository roleRepository, IFileUploadService fileUploadService, ISkillRepository skillRepository, ICandidateApplicationRepository candidateApplicationRepository, IPositionRepository positionRepository, IBatchRepository batchRepository)
        {
            _candidateRepository = candidateRepository;
            _emailService = emailService;
            _roleRepository = roleRepository;
            _fileUploadService = fileUploadService;
            _skillRepository = skillRepository;
            _candidateApplicationRepository = candidateApplicationRepository;
            _positionRepository = positionRepository;
            _batchRepository = batchRepository;
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

            var can = await _candidateRepository.GetById(jobApplicationDTO.CandidateId);
            var job = await _positionRepository.GetPositionByIdAsync(jobApplicationDTO.PositionId);
            var candidateApplication = new CandidateApplication
            {
                Candidate = can,
                Position = job,
                IsOnHold = false,
                OnHoldReason = string.Empty
            };

            await _candidateApplicationRepository.CreateApplication(candidateApplication);
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

        [HttpPost("bulk-upload")]
        public async Task<IActionResult> BulkUpload (List<IFormFile> files)
        {
            var batch = new BulkUploadBatch()
            {
                TotalFiles = files.Count,
                CreatedAt = DateTime.Now,
            };
            await _batchRepository.CreateBatch(batch);

            foreach (var file in files)
            {
                var path = await _fileUploadService.UploadFileAsync(file);
                var job = new ResumeJob()
                {
                    BatchId = batch.Id,
                    FilePath = path,
                    Status = "Pending"
                };
                await _batchRepository.AddJob(job);

                BackgroundJob.Enqueue<ResumeJobService>(x => x.ProcessResume(job.Id));
            }

            return Ok(new { batchId = batch.Id });


        }
        [HttpGet("{candidateId}/applications")]
        public async Task<IActionResult> GetCandidateApplications([FromRoute]int candidateId)
        {
            var applications = await _candidateApplicationRepository.GetApplicationsByCandidateIdAsync(candidateId);

            if (applications == null || !applications.Any())
                return NotFound(new { message = "No applications found for this candidate." });

            return Ok(applications);
        }
    }
}
