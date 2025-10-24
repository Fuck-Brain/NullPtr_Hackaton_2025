using Back.Domain.Entity;
using Back.Domain.Interfaces;
using Back.Infrastructure;

namespace Back.Application;

public class ResultRequestServices
{
    private readonly UnitOfWork unit;
    
    public ResultRequestServices(UnitOfWork unit)
    {
        this.unit = unit;
    }

    public async Task<List<User>> GetUserResultRequestRecommendations(Guid userId)
    {
        return (await unit.Result.GetAllResultRequests()).Where(r => r.UserRequestId == userId)
            .SelectMany(r => r.ResultRequestUsers ?? Enumerable.Empty<User>()) 
            .DistinctBy(u => u.Id)                                 
            .ToList();
    }
}