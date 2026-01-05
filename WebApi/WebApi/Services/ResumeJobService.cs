using Hangfire;
using Microsoft.EntityFrameworkCore;
using WebApi.ApiClient;
using WebApi.DTO;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Services
{
    public class ResumeJobService
    {
        private readonly AppDbContext _con;
        private readonly FastApiClient _fastApiClient;
        private readonly IEmailService _emailService;
        private readonly ISkillRepository _skillRepository;
        private static SemaphoreSlim _semaphore = new(5);

        public ResumeJobService(AppDbContext con, FastApiClient fastApiClient, IEmailService emailService, ISkillRepository skillRepository)
        {
            _con = con;
            _fastApiClient = fastApiClient;
            _emailService = emailService;
            _skillRepository = skillRepository;
        }
        [AutomaticRetry(Attempts =3)]
        public async Task ProcessResume(int jobId)
        {
            await _semaphore.WaitAsync();
            try
            {
                var job = await _con.Jobs.FirstOrDefaultAsync(rj => rj.Id == jobId);
                job.Status = "Processing";
                await _con.SaveChangesAsync();

                var extracted = await _fastApiClient.ParseResume(job.FilePath);

                var skills = await _skillRepository.GetSkillsByNamesAsync(extracted.Skills);
                var candidate = new Candidate()
                {
                    Name = extracted.Name,
                    Email = extracted.Email,
                    PhoneNumber = extracted.PhoneNumber,
                    PasswordHash = TokenGenerator.GenerateRandomizeToken(),
                    CurrentStatus = "Active",
                    ResumeUrl = job.FilePath,
                    CreatedAt = DateTime.UtcNow,
                    CandidateSkills = skills.Select(s => new CandidateSkill
                    {
                        SkillId = s.SkillId,
                        Skill = s,
                    }).ToList()
                };
                await _con.Candidates.AddAsync(candidate);
                _con.SaveChangesAsync();

                await _emailService.ActivationEmail(candidate, "url from resumjobservice.cs");

                job.Status = "Completed";
                var batch = await _con.Batches.FindAsync(job.BatchId);
                batch.Completed++;
                _con.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                var job = await _con.Jobs.FindAsync(jobId);
                job.Status = "Failed";
                job.Error = ex.Message;

                var batch = await _con.Batches.FindAsync(job.BatchId);
                batch.Failed++;

                await _con.SaveChangesAsync();

                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
