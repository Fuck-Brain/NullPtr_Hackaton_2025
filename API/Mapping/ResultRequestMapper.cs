using Back.Application;
using Back.Domain.Entity;
using Back.API.DTO.Request;
using Back.API.DTO.ResultRequest;
using Back.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using static Back.API.Routes.ApiRoutes;

namespace Back.API.Mapping;

public static class ResultRequestMapper
{
    public static void MapGetUserResultRequestRecommendations(ref WebApplication app)
    {
        app.MapGet(ResultRequestRoute + "/getUserRecommendations/{id}",
            async ([FromRoute] Guid id,
                [FromServices] ResultRequestServices rRServices) =>
            {
                var listUserBasic = await rRServices.GetUserResultRequestRecommendations(id);

                return Results.Ok(listUserBasic);
            }).Produces<List<UserBasicDto>>();
    }

    public static void MapGetRequestRecommendations(ref WebApplication app)
    {
        app.MapPost(ResultRequestRoute + "/getUserRecommendations",
            async ([FromBody] RequestRecommendationsDTO RRDTO,
                [FromServices] ResultRequestServices rRServices) =>
            {
                var listUserBasic = await rRServices.GetRequestRecommendations(RRDTO.userId, RRDTO.requestId);

                return Results.Ok(listUserBasic);
            }).Produces<List<UserBasicDto>>();
    }
}