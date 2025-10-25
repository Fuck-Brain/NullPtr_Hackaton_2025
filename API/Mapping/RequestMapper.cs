using Back.Application;
using Back.Domain.Entity;
using Back.API.DTO.Request;
using Microsoft.AspNetCore.Mvc;
using static Back.API.Routes.ApiRoutes;

namespace Back.API.Mapping;

public static class RequestMapper
{
    public static void MapPostCreateRegister(ref WebApplication app)
    {
        app.MapPost(RequestRoute + "/create",
            async ([FromBody] RequestCreateDTO requestCreateDTO,
                [FromServices] RequestServices rServices) =>
            {   
                var idRequest =
                    await rServices.CreateRequest(requestCreateDTO.userId, requestCreateDTO.name, requestCreateDTO.text);

                return Results.Created((string?)null, idRequest);
            }).Produces<Guid>(StatusCodes.Status201Created);
    }

    public static void MapGetGetUserRequests(ref WebApplication app)
    {
        app.MapGet(RequestRoute + "/getUserRequests/{id:guid}",
            async ([FromRoute] Guid id, [FromServices] RequestServices rServices) =>
            {
                var listRequests = await rServices.GetUserRequests(id);

                return Results.Ok(listRequests);
            }).Produces<Request>();
    }
}