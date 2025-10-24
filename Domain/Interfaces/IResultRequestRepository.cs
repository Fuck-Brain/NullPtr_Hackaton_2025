using Back.Domain.Entity;

namespace Back.Domain.Interfaces
{
    public interface IResultRequestRepository
    {
        public Task<Guid> AddResultRequest(ResultRequest resultRequest);
        public Task<ResultRequest?> GetResultRequest(Guid id);
        public Task<IEnumerable<ResultRequest>> GetAllResultRequests();
        public Task UpdateResultRequest(ResultRequest updated);
        public Task DeleteResultRequest(Guid id);
    }
}
