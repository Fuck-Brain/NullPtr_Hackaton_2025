using Back.Application.Dtos;
using Back.Domain.Entity;
using Back.Domain.Interfaces;
using Back.Infrastructure;
using Back.Infrastructure.MLClient;

namespace Back.Application;

public class ResultRequestServices
{
    private readonly UnitOfWork unit;
    private readonly MLClient _client;
    
    public ResultRequestServices(UnitOfWork unit, MLClient mlClient)
    {
        this.unit = unit;
        _client = mlClient;
    }

    // maybe useless
    public async Task<List<UserBasicDto>> GetUserResultRequestRecommendations(Guid userId)
    {
        return (await unit.Result.GetAllResultRequests()).Where(r => r.UserRequestId == userId)
            .SelectMany(r => r.ResultRequestUsers ?? Enumerable.Empty<User>()) 
            .DistinctBy(u => u.Id)
            .Select(usr =>
            {
                return new UserBasicDto()
                {
                    Age = usr.Age,
                    Gender = usr.Gender,
                    City = usr.City,
                    DescribeUser = usr.DescribeUser,
                    Name = usr.Name,
                    FatherName = usr.FatherName,
                    Hobbies = usr.Hobbies.Select(h => h.HobbyName).ToList(),
                    Login = usr.Login,
                    Id = usr.Id,
                    Interests = usr.Interests.Select(i => i.InterestName).ToList(),
                    PhotoHash = usr.PhotoHash,
                    Skills = usr.Skills.Select(s => s.SkillName).ToList(),
                    SurName = usr.SurName
                };
            })                                 
            .ToList();
    }

    public async Task<List<UserBasicDto>> GetRequestRecommendations(Guid userId, Guid requestId)
    {
        var results = await _client.GetRecommendedUsersAsync(requestId, userId);
        
        return results.Select(usr =>
        {
            return new UserBasicDto()
            {
                Age = usr.Age,
                Gender = usr.Gender,
                City = usr.City,
                DescribeUser = usr.DescribeUser,
                Name = usr.Name,
                FatherName = usr.FatherName,
                Hobbies = usr.Hobbies.Select(h => h.HobbyName).ToList(),
                Login = usr.Login,
                Id = usr.Id,
                Interests = usr.Interests.Select(i => i.InterestName).ToList(),
                PhotoHash = usr.PhotoHash,
                Skills = usr.Skills.Select(s => s.SkillName).ToList(),
                SurName = usr.SurName
            };
        }).ToList();
    }
}