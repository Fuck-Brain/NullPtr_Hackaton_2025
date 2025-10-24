using Back.Domain.Entity;

namespace Back.Domain.Interfaces
{
    public interface IRequestRepository
    {
        public Task<Guid> AddRequest(Request request);
        public Task<Request> GetRequest(Guid id);
        public Task<IEnumerable<Request>> GetAllRequests();
        public Task UpdateRequest(Request request);
        public Task DeleteRequest(Guid id);
    }
}
