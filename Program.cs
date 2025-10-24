using Back.Application;
using Back.Infrastructure.DataBase;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Back.Domain.Interfaces;
using Back.Infrastructure.Repository;

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

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
