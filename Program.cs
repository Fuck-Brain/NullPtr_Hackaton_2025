using Back.Application;
using Back.Domain.Interfaces;
using Back.Infrastructure;
using Back.Infrastructure.DataBase;
using Back.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
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

app.Run();
