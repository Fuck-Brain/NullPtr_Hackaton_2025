using Back.Application.Dtos;
using Back.Application.Exceptions;
using Back.Domain.Entity;
using Back.Domain.Interfaces;

namespace Back.Application;

public class RequestServices
{
    private readonly IRequestRepository _requestRepository;
    private readonly AnalyticsClient _analyticsClient;
    private readonly IUserRepository _userRepository;
    private readonly IResultRequestRepository _resultRepository;
    public RequestServices(IRequestRepository repository, AnalyticsClient analyticsClient, IUserRepository userRepository, IResultRequestRepository resultRequestRepository)
    {
        _requestRepository = repository;
        _analyticsClient = analyticsClient;
        _userRepository = userRepository;
        _resultRepository = resultRequestRepository;
    }

    public async Task<Guid> CreateRequest(Guid userId, string name, string text)
    {
        var user = _userRepository.GetUser(userId);
        if (user is null)
            throw new AuthException();
        
        var request = new Request(userId,  name, text);
        _requestRepository.AddRequest(request);
        var response = await _analyticsClient.ClassifyAsync(new ClassificationRequest(request, _userRepository.GetAllUser().ToList()));
        var requestResult = new ResultRequest(request, user, response.ClassifiedUsers);
        // TODO: save
        return request.Id;
    }

    public List<Request> GetUserRequests(Guid userId)
    {
        return _requestRepository.GetAllRequests().Where(x => x.UserId == userId).ToList();
    }
}