using Back.Application;
using Back.API.DTO;
using Back.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Back.API.Mapping;

public static class UserMapper
{
    public static void MapPostRegister(ref WebApplication app)
    {
        app.MapPost("/register", async ([FromBody] UserDTO userDTO, [FromServices] UserServices uServices) =>
        {
            var token = await uServices.Register(userDTO.Login, userDTO.Password, userDTO.PhotoHash,
                userDTO.Name,
                userDTO.SurName,
                userDTO.FatherName, userDTO.Age, userDTO.Gender, userDTO.City, userDTO.Contact);

            return Results.Created((string?)null, token);
        });
    }

    public static void MapGetLogin(ref WebApplication app)
    {
        app.MapGet("/login", async ([FromBody] UserDTO userDTO, [FromServices] UserServices uServices) =>
        {
            var token = await uServices.Login(userDTO.Login, userDTO.Password);

            return Results.Ok(token);
        });
    }

    public static void MapPutUpdate(ref WebApplication app)
    {
        app.MapPut("/update", async ([FromBody] UserDTO userDTO, [FromServices] UserServices uServices) =>
        {
            if (userDTO.Id is null)
                return Results.BadRequest("Id is null");

            await uServices.Update(userDTO.Id!.Value, new UserUpdateDto()
            {
                Login = userDTO.Login,
                Password = userDTO.Password,
                Name = userDTO.Name,
                Surname = userDTO.SurName,
                FatherName = userDTO.FatherName,
                Age = userDTO.Age,
                Gender = userDTO.Gender,
                City = userDTO.City,
                Contact = userDTO.Contact,
                //Skills = userDTO.Skills, // TODO: list of skills
                Description = userDTO.DescribeUser
            });

            return Results.Ok();
        });
    }

    public static void MapPostLikeDislike(ref WebApplication app)
    {
        app.MapPost("/{id:guid}/like",
            async ([FromRoute] Guid id, [FromBody] Guid idUserFrom, [FromServices] UserServices uServices) =>
            {
                await uServices.LikeUser(idUserFrom, id);

                return Results.Ok();
            }
        );

        app.MapPost("/{id:guid}/dislike",
            async ([FromRoute] Guid id, [FromBody] Guid idUserFrom, [FromServices] UserServices uServices) =>
            {
                await uServices.DislikeUser(idUserFrom, id);
                
                return Results.Ok();
            });
    }
}