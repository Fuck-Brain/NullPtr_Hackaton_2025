using Back.Application.Dtos;
using Back.Application.Exceptions;
using Back.Domain.Entity;
using Back.Domain.Interfaces;
using Back.Infrastructure.MLClient;

namespace Back.Application;

public class RequestServices
{
    private readonly IRequestRepository _requestRepository;
    private readonly AnalyticsClient _analyticsClient;
    private readonly IUserRepository _userRepository;
    private readonly IResultRequestRepository _resultRepository;
    private readonly MLClient _client;
    public RequestServices(IRequestRepository repository, AnalyticsClient analyticsClient, IUserRepository userRepository, IResultRequestRepository resultRequestRepository, MLClient client)
    {
        _requestRepository = repository;
        _analyticsClient = analyticsClient;
        _userRepository = userRepository;
        _resultRepository = resultRequestRepository;
        _client = client;
    }

    public async Task<Guid> CreateRequest(Guid userId, string name, string text)
    {
        var user = await _userRepository.GetUser(userId);
        if (user is null)
            throw new AuthException();
        
        var request = new Request(userId,  name, text);

        await _requestRepository.AddRequest(request);
        await _client.ProcessRequestAsync(request.Id);
        return request.Id;
    }
    
    public async Task<List<RequestDto>> GetUserRequests(Guid userId)
    {
        return (await _requestRepository.GetAllRequests()).Where(x => x.UserId == userId).Select(r => new RequestDto()
        {
            Id = r.Id,
            Name = r.NameRequest,
            Text = r.TextRequest
        }).ToList();
    }
}