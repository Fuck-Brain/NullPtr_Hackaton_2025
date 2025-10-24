using Back.Domain.Entity;
using Back.Domain.Interfaces;

namespace Back.Application;

public class ResultRequestServices
{
    private readonly IResultRequestRepository _resultRequestRepository;
    
    public ResultRequestServices(IResultRequestRepository resultRequestRepository)
    {
        _resultRequestRepository = resultRequestRepository;
    }

    public async Task<User> GetUserResultRequestRecommendations(int userId)
    {
        throw new NotImplementedException();
        //_resultRequestRepository.
    }
}