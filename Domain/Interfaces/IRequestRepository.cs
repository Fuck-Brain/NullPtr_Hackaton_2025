using Back.Domain.Entity;

namespace Back.Domain.Interfaces
{
    public interface IRequestRepository
    {
        public Guid AddRequest(Request request);
        public Request GetRequest(Guid id);
        public IEnumerable<Request> GetAllRequests();
        public void UpdateRequest(Request request);
        public void DeleteRequest(Guid id);
    }
}
