using Back.Application;
using Back.Domain.Interfaces;
using Back.Infrastructure;
using Back.Infrastructure.DataBase;
using Back.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Back.Domain.Entity;
using Back.API.DTO;
using Back.API.Mapping;
using UserMapper = Back.API.Mapping.UserMapper;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<AnalyticsClient>(c =>
{
    c.BaseAddress = new Uri("http://localhost:8000/");
    c.Timeout = TimeSpan.FromSeconds(10);
});
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

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    bool seedEnabled = true; // �������� ��� ���������� ��������� �������
    await DbInitializer.EnsureCreatedAndSeedAsync(db, seedEnabled);
}
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.MapGet("/", () => "Hello World!");

// User
UserMapper.MapPostRegister(ref app);
UserMapper.MapGetLogin(ref app);
UserMapper.MapPutUpdate(ref app);
UserMapper.MapPostLikeDislike(ref app);

// R

app.Run();