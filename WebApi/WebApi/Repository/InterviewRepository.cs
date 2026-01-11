using Microsoft.EntityFrameworkCore;
using WebApi.Enums;
using WebApi.Models;

namespace WebApi.Repository
{
    public interface IInterviewRepository
    {
        Task<InterviewRound> ScheduleInterviewAsync(InterviewRound round, List<int> interviewerIds);
        Task<Interview> GetInterviewByIdAsync(int interviewId);
        Task SubmitFeedbackAsync(Interview interview, List<InterviewRating> ratings);
        Task CompleteRoundAsync(int roundId, InterviewResult result);
        Task<InterviewRound> GetInterviewRoundByIdAsync(int roundId);
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
            List<InterviewRating> ratings)
        {
            interview.CompletedAt = DateTime.UtcNow;

            _context.InterviewRatings.AddRange(ratings);
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
    }
}
