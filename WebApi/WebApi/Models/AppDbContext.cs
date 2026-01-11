using Microsoft.EntityFrameworkCore;

namespace WebApi.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-many Position and Skill
            modelBuilder.Entity<PositionSkill>()
                .HasKey(ps => new { ps.PositionId, ps.SkillId });

            modelBuilder.Entity<PositionSkill>()
                .HasOne(ps => ps.Position)
                .WithMany(jp => jp.PositionSkills)
                .HasForeignKey(ps => ps.PositionId);

            modelBuilder.Entity<PositionSkill>()
                .HasOne(ps => ps.Skill)
                .WithMany(s => s.PositionSkills)
                .HasForeignKey(ps => ps.SkillId);

            // many-to-many Candidate and Skill
            modelBuilder.Entity<CandidateSkill>()
                .HasKey(cs => new { cs.CandidateId, cs.SkillId });

            modelBuilder.Entity<CandidateSkill>()
                .HasOne(cs => cs.Candidate)
                .WithMany(c => c.CandidateSkills)
                .HasForeignKey(cs => cs.CandidateId);

            modelBuilder.Entity<CandidateSkill>()
                .HasOne(cs => cs.Skill)
                .WithMany(s => s.CandidateSkills)
                .HasForeignKey(cs => cs.SkillId);

            modelBuilder.Entity<InterviewRating>()
                .HasKey(ir => new { ir.InterviewId, ir.SkillId });

            modelBuilder.Entity<InterviewRating>()
                .HasOne(ir => ir.Interview)
                .WithMany(i => i.InterviewRatings)
                .HasForeignKey(ir => ir.InterviewId);

            modelBuilder.Entity<InterviewRating>()
                .HasOne(ir => ir.Skill)
                .WithMany(s => s.InterviewRatings)
                .HasForeignKey(ir => ir.SkillId);

            modelBuilder.Entity<OfferLetter>()
            .HasOne(o => o.SentByUser)  
            .WithMany()  
            .HasForeignKey(o => o.SentByUserId)  
            .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Interview>()
           .HasOne(i => i.Interviewer)  
           .WithMany(u => u.Interviews)  
           .HasForeignKey(i => i.InterviewerId)  
           .OnDelete(DeleteBehavior.NoAction);

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Position> Positions { get; set; }  
        public DbSet<Role> Roles { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<PositionSkill> PositionSkills { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<CandidateSkill> CandidateSkills { get; set; }
        public DbSet<CandidateApplication> CandidateApplications { get; set; }
        public DbSet<InterviewRound> InterviewRounds { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<InterviewRating> InterviewRatings { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<CandidateDocument> CandidateDocuments { get; set; }
        public DbSet<OfferLetter> OfferLetters { get; set; }
        public DbSet<BulkUploadBatch> Batches { get; set; }
        public DbSet<ResumeJob> Jobs { get; set; }
        public DbSet<RoundType> RoundTypes { get; set; }
    }
}
