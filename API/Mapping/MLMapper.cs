using Back.API.DTO;
using Back.Application;
using Back.Domain.Entity;
using Back.API.DTO.Request;
using Back.API.DTO.ResultRequest;
using Back.API.Routes.DTO.ML;
using Back.Application.Dtos;
using Back.Infrastructure.MLClient;
using Microsoft.AspNetCore.Mvc;
using static Back.API.Routes.ApiRoutes;

namespace Back.API.Mapping;

public static class MLMapper
{
    public static void MapGetGetRecommendedUsers(ref WebApplication app)
    {
        app.MapPost(MLRoute + "/getRecommendedUsers",
            async ([FromBody] UserRequestDTO urDTO, [FromServices] MLClient ml) =>
            {
                var listUser = (await ml.GetRecommendedUsersAsync(urDTO.requestId, urDTO.userId)).Select(user => new UserDetailsDto
                {
                    Id = user.Id,
                    Login = user.Login,
                    PhotoHash = user.PhotoHash,
                    Name = user.Name,
                    SurName = user.SurName,
                    FatherName = user.FatherName,
                    Age = user.Age,
                    Gender = user.Gender,
                    DescribeUser = user.DescribeUser,
                    City = user.City,
                    Contact = user.Contact,
                    Skills = user.Skills.Select(s => s.SkillName).ToList(),
                    Hobbies = user.Hobbies.Select(s => s.HobbyName).ToList(),
                    Interests = user.Interests.Select(s => s.InterestName).ToList(),
                });

                return Results.Ok(listUser);
            }).Produces<IEnumerable<User>>();
    }

    public static void MapGetGetRequestsFrequencyStatistics(ref WebApplication app)
    {
        app.MapPost(MLRoute + "/getRequestsFrequencyStatistics",
            async ([FromBody] FilterOptions fOp, [FromServices] MLClient ml) =>
            {
                var frequencyStatDict = await ml.GetRequestsFrequencyStatisticsAsync(fOp);
                frequencyStatDict = frequencyStatDict.OrderByDescending(x => x.Value).ToDictionary();

                return Results.Ok(frequencyStatDict);
            }).Produces<Dictionary<string, int>>();
    }

    public static void MapGetGetMostPopularSkills(ref WebApplication app)
    {
        app.MapPost(MLRoute + "/getMostPopularSkills",
            async ([FromBody] FilterOptions fOp, [FromServices] MLClient ml) =>
            {
                var mostPopularSkills = await ml.GetMostPopularSkillsAsync(fOp);
                mostPopularSkills = mostPopularSkills.OrderByDescending(x => x.Value).ToDictionary();

                return Results.Ok(mostPopularSkills);
            }).Produces<Dictionary<string, int>>();
    }

    public static void MapGetGetMostPopularHobby(ref WebApplication app)
    {
        app.MapPost(MLRoute + "/getMostPopularHobby",
            async ([FromBody] FilterOptions fOp, [FromServices] MLClient ml) =>
            {
                var mostPopularHobby = await ml.GetMostPopularHobbyAsync(fOp);
                mostPopularHobby = mostPopularHobby.OrderByDescending(x => x.Value).ToDictionary();

                return Results.Ok(mostPopularHobby);
            }).Produces<Dictionary<string, int>>();
    }

    public static void MapGetGetMostPopularInterest(ref WebApplication app)
    {
        app.MapPost(MLRoute + "/getMostPopularInterest",
            async ([FromBody] FilterOptions fOp, [FromServices] MLClient ml) =>
            {
                var mostPopularInterest = await ml.GetMostPopularInterestAsync(fOp);
                mostPopularInterest = mostPopularInterest.OrderByDescending(x => x.Value).ToDictionary();

                return Results.Ok(mostPopularInterest);
            }).Produces<Dictionary<string, int>>();
    }
}