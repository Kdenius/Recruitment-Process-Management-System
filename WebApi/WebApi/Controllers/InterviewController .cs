using Microsoft.AspNetCore.Mvc;
using WebApi.DTO;
using WebApi.Enums;
using WebApi.Models;
using WebApi.Repository;
using WebApi.Services;
using static System.Net.Mime.MediaTypeNames;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("interviews")]
    public class InterviewController : ControllerBase
    {
        private readonly IInterviewRepository _repo;
        private readonly IEmailService _emailService;
        private readonly ICandidateApplicationRepository _candidateApplicationRepository;

        public InterviewController(IInterviewRepository repo, IEmailService emailService, ICandidateApplicationRepository candidateApplicationRepository)
        {
            _repo = repo;
            _emailService = emailService;
            _candidateApplicationRepository = candidateApplicationRepository;
        }

        [HttpPost("schedule")]
        public async Task<IActionResult> ScheduleInterview(ScheduleInterviewDTO dto)
        {
            var app = await _candidateApplicationRepository.GetApplicationsByIdAsync(dto.ApplicationId);
            if (app == null)
                throw new Exception("Application not found");
            var round = new InterviewRound
            {
                ApplicationId = dto.ApplicationId,
                RoundTypeId = dto.RoundTypeId,
                RoundNumber = dto.RoundNumber,
                ScheduledAt = dto.ScheduledAt,
                Mode = dto.Mode,
                Location = dto.Location,
                MeetingLink = dto.MeetingLink
            };

            await _repo.ScheduleInterviewAsync(round, dto.InterviewerIds);

            var rnd = await _repo.GetInterviewRoundByIdAsync(round.RoundId);

            var locOrLink = dto.Mode == InterviewMode.Offline ? dto.Location : dto.MeetingLink;
            var mod = dto.Mode == InterviewMode.Offline ? "Offline" : "Online";

           await _emailService.InterviewScheduled(rnd.CandidateApplication.Candidate.Name, rnd.CandidateApplication.Candidate.Email, rnd.CandidateApplication.Position.Title, rnd.RoundType.TypeName, dto.ScheduledAt, mod, locOrLink);

            return Ok(new { message = "Interview scheduled successfully" });
        }

        [HttpPost("feedback")]
        public async Task<IActionResult> SubmitFeedback(InterviewFeedbackDTO dto)
        {
            var interview = await _repo.GetInterviewByIdAsync(dto.InterviewId);

            interview.FeedbackText = dto.FeedbackText;
            interview.FeedbackScore = dto.FeedbackScore;

            var ratings = dto.Ratings.Select(r => new InterviewRating
            {
                InterviewId = interview.InterviewId,
                SkillId = r.SkillId,
                Rating = r.Rating,
                Remark = r.Remark
            }).ToList();

            await _repo.SubmitFeedbackAsync(interview, ratings);

            return Ok(new { message = "Feedback submitted" });
        }

        [HttpPost("decision")]
        public async Task<IActionResult> FinalDecision(InterviewDecisionDTO dto)
        {
            await _repo.CompleteRoundAsync(dto.RoundId, dto.Result);
            var rnd = await _repo.GetInterviewRoundByIdAsync(dto.RoundId);
            var res = dto.Result == InterviewResult.Selected ? "Selected" : "Rejected";
            await _emailService.InterviewResult(rnd.CandidateApplication.Candidate.Name, rnd.CandidateApplication.Candidate.Email, rnd.CandidateApplication.Position.Title, rnd.RoundType.TypeName, res, dto.Remark);
            return Ok(new { message = "Decision saved" });
        }
    }

}
