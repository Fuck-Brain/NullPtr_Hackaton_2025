using Back.Domain.Entity;
using Back.Domain.Interfaces;

namespace Back.Application;

public class RequestServices
{
    private readonly IRequestRepository _requestRepository;
    public RequestServices(IRequestRepository repository)
    {
        _requestRepository = repository;
    }

    public Guid CreateRequest(Guid userId, string name, string text)
    {
        var request = new Request(userId,  name, text);
        _requestRepository.AddRequest(request);
        // TODO: send req to ml
        return request.Id;
    }

    public List<Request> GetUserRequests(Guid userId)
    {
        return _requestRepository.GetAllRequests().Where(x => x.UserId == userId).ToList();
    }
}