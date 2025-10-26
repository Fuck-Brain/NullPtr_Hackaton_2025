using Swashbuckle.AspNetCore.Filters;
using Back.API.DTO;
using Back.API.DTO.Request;
using Back.API.DTO.ResultRequest;
using Back.API.Routes.DTO.ML;
using Back.Application;

namespace Back.API.SwaggerExamples;

public class UserLoginDtoExample : IExamplesProvider<UserLoginDto>
{
    public UserLoginDto GetExamples() => new()
    {
        Login = "test_user",
        Password = "Pa$$w0rd"
    };
}

public class UserRegisterDtoExample : IExamplesProvider<UserRegisterDTO>
{
    public UserRegisterDTO GetExamples() => new()
    {
        Login = "test_user",
        Password = "Pa$$w0rd",
        PhotoHash = "photo_hash",
        Name = "Иван",
        SurName = "Иванов",
        FatherName = "Иванович",
        Age = 25,
        Gender = "Мужской",
        City = "Москва",
        Contact = "+7 (999) 123-45-67"
    };
}

public class AuthResponseDtoExample : IExamplesProvider<AuthResponseDto>
{
    public AuthResponseDto GetExamples() => new()
    {
        UserId = Guid.NewGuid(),
        Token = "eyJhbGciOiJIUzI1NiIsInR5..."
    };
}

public class UserDtoExample : IExamplesProvider<UserDTO>
{
    public UserDTO GetExamples() => new()
    {
        Id = Guid.NewGuid(),
        Login = "test_user",
        Password = "Pa$$w0rd",
        PhotoHash = "hash123",
        Name = "Иванов",
        SurName = "Иван",
        FatherName = "Иванович",
        Age = 29,
        Gender = "Мужской",
        DescribeUser = "Backend-разработчик",
        City = "Минск",
        Contact = "ivanov@example.com"
    };
}

public class UserUpdateDtoExample : IExamplesProvider<UserUpdateDTO>
{
    public UserUpdateDTO GetExamples() => new()
    {
        Id = Guid.NewGuid(),
        Login = "updated_user",
        Password = "newpassword",
        Name = "Павел",
        Surname = "Смирнов",
        FatherName = "Алексеевич",
        Age = 31,
        Gender = "Мужской",
        City = "Санкт-Петербург",
        Contact = "+7 (901) 555-11-22",
        Skills = new() { "C#", ".NET", "SQL" },
        Interests = new() { "AI", "ML" },
        Hobbies = new() { "Плавание", "Гитара" },
        Description = "Опытный разработчик с интересом к ML"
    };
}

public class RequestCreateDtoExample : IExamplesProvider<RequestCreateDTO>
{
    public RequestCreateDTO GetExamples() => new()
    {
        userId = Guid.NewGuid(),
        name = "Тестовый запрос",
        text = "Описание запроса для тестирования системы"
    };
}

public class RequestRecommendationsDtoExample : IExamplesProvider<RequestRecommendationsDTO>
{
    public RequestRecommendationsDTO GetExamples() => new()
    {
        userId = Guid.NewGuid(),
        requestId = Guid.NewGuid()
    };
}

public class UserRequestDtoExample : IExamplesProvider<UserRequestDTO>
{
    public UserRequestDTO GetExamples() => new()
    {
        userId = Guid.NewGuid(),
        requestId = Guid.NewGuid()
    };
}