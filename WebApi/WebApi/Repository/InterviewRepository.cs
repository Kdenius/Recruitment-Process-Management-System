using Microsoft.EntityFrameworkCore;
using WebApi.DTO;
using WebApi.Enums;
using WebApi.Models;

namespace WebApi.Repository
{
    public interface IInterviewRepository
    {
        Task<InterviewRound> ScheduleInterviewAsync(InterviewRound round, List<int> interviewerIds);
        Task<Interview> GetInterviewByIdAsync(int interviewId);
        Task SubmitFeedbackAsync(Interview interview, InterviewFeedbackDTO dto);
        Task CompleteRoundAsync(int roundId, InterviewResult result);
        Task<InterviewRound> GetInterviewRoundByIdAsync(int roundId);

        Task<IEnumerable<InterviewRound>> GetRoundsByInterviewerIdAsync(int interviewerId);

        Task<IEnumerable<Interview>> GetInterviewsByInterviewerIdAsync(int interviewerId);
    }
    public class InterviewRepository : IInterviewRepository
    {
        private readonly AppDbContext _context;

        public InterviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<InterviewRound> ScheduleInterviewAsync(
            InterviewRound round,
            List<int> interviewerIds)
        {
            _context.InterviewRounds.Add(round);
            await _context.SaveChangesAsync();

            foreach (var interviewerId in interviewerIds)
            {
                _context.Interviews.Add(new Interview
                {
                    RoundId = round.RoundId,
                    InterviewerId = interviewerId
                });
            }

            await _context.SaveChangesAsync();
            return round;
        }

        public async Task<Interview> GetInterviewByIdAsync(int interviewId)
        {
            return await _context.Interviews
                .Include(i => i.InterviewRatings)
                .FirstOrDefaultAsync(i => i.InterviewId == interviewId);
        }

        public async Task SubmitFeedbackAsync(
    Interview interview,
    InterviewFeedbackDTO dto)
        {
            // Update interview feedback
            interview.FeedbackText = dto.FeedbackText;
            interview.FeedbackScore = dto.FeedbackScore;
            interview.CompletedAt ??= DateTime.UtcNow;

            // Load existing ratings
            var existingRatings = await _context.InterviewRatings
                .Where(r => r.InterviewId == interview.InterviewId)
                .ToListAsync();

            var incomingRatings = dto.Ratings ?? new List<InterviewRatingDTO>();

            // UPDATE or INSERT
            foreach (var ratingDto in incomingRatings)
            {
                var existing = existingRatings
                    .FirstOrDefault(r => r.SkillId == ratingDto.SkillId);

                if (existing != null)
                {
                    // Update
                    existing.Rating = ratingDto.Rating;
                    existing.Remark = ratingDto.Remark;
                }
                else
                {
                    // Insert
                    _context.InterviewRatings.Add(new InterviewRating
                    {
                        InterviewId = interview.InterviewId,
                        SkillId = ratingDto.SkillId,
                        Rating = ratingDto.Rating,
                        Remark = ratingDto.Remark
                    });
                }
            }

            // DELETE ratings removed by client (optional but recommended)
            var skillIdsFromClient = incomingRatings.Select(r => r.SkillId).ToHashSet();

            var ratingsToRemove = existingRatings
                .Where(r => !skillIdsFromClient.Contains(r.SkillId))
                .ToList();

            if (ratingsToRemove.Any())
                _context.InterviewRatings.RemoveRange(ratingsToRemove);

            _context.Interviews.Update(interview);

            await _context.SaveChangesAsync();
        }


        public async Task CompleteRoundAsync(int roundId, InterviewResult result)
        {
            var round = await _context.InterviewRounds.FindAsync(roundId);

            round.Result = result;
            round.IsCompleted = true;

            await _context.SaveChangesAsync();
        }
        public async Task<InterviewRound> GetInterviewRoundByIdAsync(int roundId)
        {
            var ret = await _context.InterviewRounds
            .Include(r => r.RoundType)

            .Include(r => r.CandidateApplication)
                .ThenInclude(a => a.Candidate)

            .Include(r => r.CandidateApplication)
                .ThenInclude(a => a.Position)

            .Include(r => r.Interviews)
                .ThenInclude(i => i.Interviewer)

            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.RoundId == roundId);
            return ret;
        }

        public async Task<IEnumerable<InterviewRound>> GetRoundsByInterviewerIdAsync(int interviewerId)
        {
            return await _context.InterviewRounds
                .Include(r => r.RoundType)
                .Include(r => r.CandidateApplication)
                    .ThenInclude(a => a.Candidate)
                .Include(r => r.CandidateApplication)
                    .ThenInclude(a => a.Position)
                .Include(r => r.Interviews)
                .Where(r => r.Interviews.Any(i => i.InterviewerId == interviewerId))
                .OrderByDescending(r => r.ScheduledAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Interview>> GetInterviewsByInterviewerIdAsync(int interviewerId)
        {
            return await _context.Interviews
                .Where(i => i.InterviewerId == interviewerId)

                .Include(i => i.InterviewRound)
                    .ThenInclude(r => r.RoundType)

                .Include(i => i.InterviewRound)
                    .ThenInclude(r => r.CandidateApplication)
                        .ThenInclude(a => a.Candidate)

                .Include(i => i.InterviewRound)
                    .ThenInclude(r => r.CandidateApplication)
                        .ThenInclude(a => a.Position)
                .Include(i => i.InterviewRatings)
                    .ThenInclude(ir => ir.Skill)

                .OrderByDescending(i => i.InterviewRound.ScheduledAt)

                .ToListAsync();
        }

    }
}
