using Back.Application;
using Back.API.DTO;
using Back.Application.Dtos;
using Back.Application.Exceptions;
using Back.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Back.API.Routes.ApiRoutes;

namespace Back.API.Mapping;

public static class UserMapper
{
    public static void MapPostRegister(ref WebApplication app)
    {
        app.MapPost(UserRoute + "/register",
            async ([FromBody] UserRegisterDTO userDTO, [FromServices] UserServices uServices) =>
            {
                var token = await uServices.Register(userDTO.Login, userDTO.Password, userDTO.PhotoHash,
                    userDTO.Name,
                    userDTO.SurName,
                    userDTO.FatherName, userDTO.Age, userDTO.Gender, userDTO.City, userDTO.Contact);

                return Results.Created((string?)null, token);
            }).Produces<AuthResponseDto>(StatusCodes.Status201Created);
    }

    public static void MapGetLogin(ref WebApplication app)
    {
        app.MapPost(UserRoute + "/login", async ([FromBody] UserLoginDto userDTO, [FromServices] UserServices uServices) =>
        {
            var token = await uServices.Login(userDTO.Login, userDTO.Password);

            return Results.Ok(token);
        }).Produces<AuthResponseDto>(StatusCodes.Status200OK);
    }

    public static void MapPutUpdate(ref WebApplication app)
    {
        app.MapPut(UserRoute + "/update", async ([FromBody] UserUpdateDTO userDTO, [FromServices] UserServices uServices) =>
        {
            await uServices.Update(userDTO.Id, new UserUpdateDto()
            {
                Login = userDTO.Login,
                Password = userDTO.Password,
                Name = userDTO.Name,
                Surname = userDTO.Surname,
                FatherName = userDTO.FatherName,
                Age = userDTO.Age,
                Gender = userDTO.Gender,
                City = userDTO.City,
                Contact = userDTO.Contact,
                //Skills = userDTO.Skills, // TODO: list of skills
                Description = userDTO.Description,
                Skills = userDTO.Skills,
                Interests = userDTO.Interests,
                Hobbies = userDTO.Hobbies
            });

            return Results.Ok();
        }).RequireAuthorization();
    }

    public static void MapGetMe(ref WebApplication app)
    {
        app.MapGet(UserRoute + "/{id:guid}/me", async ([FromRoute] Guid id, [FromServices] UserServices uServices) =>
        {
            var user = await uServices.Me(id);

            return Results.Ok(user);
        }).Produces<UserDetailsDto>(StatusCodes.Status200OK);
    }

    public static void MapGetGetUser(ref WebApplication app)
    {
        app.MapGet(UserRoute + "/{id:guid}", async ([FromRoute] Guid id, [FromServices] UserServices uServices) =>
        {
            var userBasic = await uServices.GetUser(id);

            return Results.Ok(userBasic);
        }).Produces<UserBasicDto>(StatusCodes.Status200OK);
    }

    public static void MapPostLikeUnlike(ref WebApplication app)
    {
        app.MapPost(UserRoute + "/{id:guid}/like",
            async ([FromRoute] Guid id, [FromBody] Guid idUserFrom, [FromServices] UserServices uServices) =>
            {
                await uServices.LikeUser(idUserFrom, id);

                return Results.Ok();
            }
        ).RequireAuthorization();

        app.MapPost(UserRoute + "/{id:guid}/unlike",
            async ([FromRoute] Guid id, [FromBody] Guid idUserFrom, [FromServices] UserServices uServices) =>
            {
                await uServices.UnLikeUser(idUserFrom, id);

                return Results.Ok();
            }).RequireAuthorization();
    }

    public static void MapGetGetLiked(ref WebApplication app)
    {
        app.MapGet(UserRoute + "/{id:guid}/getLiked",
            async ([FromRoute] Guid id, [FromServices] UserServices uServices) =>
            {
                var list = await uServices.GetLiked(id);

                return Results.Ok(list);
            }).Produces<List<UserBasicDto>>();
    }

    public static void MapGetGetHasLiked(ref WebApplication app)
    {
        app.MapGet(UserRoute + "/{id:guid}/hasLiked",
            async ([FromRoute] Guid id, [FromServices] UserServices uServices) =>
            {
                var listHasLiked = await uServices.GetHasLiked(id);

                return Results.Ok(listHasLiked);
            }).Produces<List<UserBasicDto>>();
    }

    public static void MapGetGetMatches(ref WebApplication app)
    {
        app.MapGet(UserRoute + "/{id:guid}/getMatches",
            async ([FromRoute] Guid id, [FromServices] UserServices uServices) =>
            {
                var listMatches = await uServices.GetMatches(id);

                return Results.Ok(listMatches);
            }).Produces<List<UserMatchDto>>();
    }
}
