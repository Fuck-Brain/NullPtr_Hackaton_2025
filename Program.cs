using Back.API.DTO;
using Back.API.Mapping;
using Back.Application;
using Back.Domain.Entity;
using Back.Domain.Interfaces;
using Back.Infrastructure;
using Back.Infrastructure.DataBase;
using Back.Infrastructure.MLClient;
using Back.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.OpenApi.Models;
using UserMapper = Back.API.Mapping.UserMapper;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserRepository, UserRepositorySqlLite>();
builder.Services.AddScoped<IRequestRepository, RequestRepositorySqlLite>();
builder.Services.AddScoped<IResultRequestRepository, ResultRequestRepositorySqlLite>();
builder.Services.AddScoped<IUserLikeRepository, UserLikeRepositorySqlLite>();
builder.Services.AddScoped<IUserHobbyRepository, UserHobbyRepositorySqlLite>();
builder.Services.AddScoped<IUserInterestRepository, UserInterestRepositorySqlLite>();
builder.Services.AddScoped<IUserSkillRepository, UserSkillRepositorySqlLite>();
builder.Services.AddScoped<UnitOfWork>();

/// Aplication Services
builder.Services.AddScoped<AnalyticsClient>();
builder.Services.AddScoped<RequestServices>();
builder.Services.AddScoped<UserServices>();

var mlServerUrl = builder.Configuration["MLServer:BaseUrl"];

builder.Services.AddHttpClient<MLClient>(client =>
{
    client.BaseAddress = new Uri(mlServerUrl ?? "http://localhost:8000/");
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    bool seedEnabled = true; // включить для заполнения тестовыми данными
    await DbInitializer.EnsureCreatedAndSeedAsync(db, seedEnabled);
}
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.MapGet("/", () => "Hello World!");
UserMapper.MapPostRegister(ref app);
UserMapper.MapGetLogin(ref app);
UserMapper.MapPutUpdate(ref app);
UserMapper.MapDelete(ref app);

app.Run();