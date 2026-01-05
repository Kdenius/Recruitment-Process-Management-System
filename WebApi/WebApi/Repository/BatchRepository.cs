using WebApi.Models;

namespace WebApi.Repository
{
    public interface IBatchRepository
    {
        Task CreateBatch(BulkUploadBatch batch);
        Task AddJob(ResumeJob job);
    }
    public class BatchRepository : IBatchRepository
    {
        private readonly AppDbContext _con;

        public BatchRepository(AppDbContext con)
        {
            _con = con;
        }
        public async Task CreateBatch(BulkUploadBatch batch)
        {
            _con.Batches.Add(batch);
            await _con.SaveChangesAsync();
        }
        public async Task AddJob(ResumeJob job) 
        { 
            _con.Jobs.Add(job);
            await _con.SaveChangesAsync();
        }
    }
}
