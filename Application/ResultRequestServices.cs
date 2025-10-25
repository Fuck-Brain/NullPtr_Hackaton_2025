using Back.Application.Dtos;
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
                    Hobbies = usr.Hobbies,
                    Login = usr.Login,
                    Id = usr.Id,
                    Interests = usr.Interests,
                    PhotoHash = usr.PhotoHash,
                    Skills = usr.Skills,
                    SurName = usr.SurName
                };
            })                                 
            .ToList();
    }

    public async Task<List<UserBasicDto>> GetRequestRecommendations(Guid requestId)
    {
        var result = (await unit.Result.GetAllResultRequests()).FirstOrDefault(r => r.RequestId == requestId);
        if (result is null)
            throw new ArgumentException("no such request");
        
        return result.ResultRequestUsers.Select(usr =>
        {
            return new UserBasicDto()
            {
                Age = usr.Age,
                Gender = usr.Gender,
                City = usr.City,
                DescribeUser = usr.DescribeUser,
                Name = usr.Name,
                FatherName = usr.FatherName,
                Hobbies = usr.Hobbies,
                Login = usr.Login,
                Id = usr.Id,
                Interests = usr.Interests,
                PhotoHash = usr.PhotoHash,
                Skills = usr.Skills,
                SurName = usr.SurName
            };
        }).ToList();
    }
}