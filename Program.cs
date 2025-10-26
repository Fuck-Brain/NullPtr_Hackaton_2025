using Back.API.DTO;
using Back.API.Mapping;
using Back.Application;
using Back.Application.Auth;
using Back.Domain.Entity;
using Back.Domain.Interfaces;
using Back.Infrastructure;
using Back.Infrastructure.DataBase;
using Back.Infrastructure.HubChat;
using Back.Infrastructure.MLClient;
using Back.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Вставьте сюда JWT без префикса 'Bearer '",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",     // Swagger сам добавит "Bearer " перед токеном
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});
builder.Services.AddAuthorization();
builder.Services.AddScoped<IUserRepository, UserRepositorySqlLite>();
builder.Services.AddScoped<IRequestRepository, RequestRepositorySqlLite>();
builder.Services.AddScoped<IResultRequestRepository, ResultRequestRepositorySqlLite>();
builder.Services.AddScoped<IUserLikeRepository, UserLikeRepositorySqlLite>();
builder.Services.AddScoped<IUserHobbyRepository, UserHobbyRepositorySqlLite>();
builder.Services.AddScoped<IUserInterestRepository, UserInterestRepositorySqlLite>();
builder.Services.AddScoped<IUserSkillRepository, UserSkillRepositorySqlLite>();
builder.Services.AddScoped<IChatRepository, ChatRepositorySqlLite>();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

/// Aplication Services
builder.Services.AddScoped<AnalyticsClient>();
builder.Services.AddScoped<RequestServices>();
builder.Services.AddScoped<UserServices>();
builder.Services.AddScoped<ResultRequestServices>();

var mlServerUrl = builder.Configuration["MLServer:BaseUrl"];

builder.Services.AddHttpClient<MLClient>(client =>
{
    client.BaseAddress = new Uri(mlServerUrl ?? "http://localhost:8000/");
});

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
                  ?? throw new InvalidOperationException("JWT configuration is missing.");
var key = Encoding.UTF8.GetBytes(jwtSettings.Secret ?? throw new InvalidOperationException("JWT secret is missing."));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = !string.IsNullOrWhiteSpace(jwtSettings.Issuer),
        ValidateAudience = !string.IsNullOrWhiteSpace(jwtSettings.Audience),
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = string.IsNullOrWhiteSpace(jwtSettings.Issuer) ? null : jwtSettings.Issuer,
        ValidAudience = string.IsNullOrWhiteSpace(jwtSettings.Audience) ? null : jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddSignalR();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    bool seedEnabled = true; // �������� ��� ���������� ��������� �������
    await DbInitializer.EnsureCreatedAndSeedAsync(db, seedEnabled);
}

app.UseRouting();
app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Hello World!");

// User
UserMapper.MapPostRegister(ref app);
UserMapper.MapGetLogin(ref app);
UserMapper.MapPutUpdate(ref app);
UserMapper.MapGetMe(ref app);

UserMapper.MapGetGetUser(ref app);
UserMapper.MapPostLikeUnlike(ref app);
UserMapper.MapGetGetLiked(ref app);
UserMapper.MapGetGetHasLiked(ref app);
UserMapper.MapGetGetMatches(ref app);

// Request
RequestMapper.MapPostCreateRegister(ref app);
RequestMapper.MapGetGetUserRequests(ref app);

// ResultRequest
// ResultRequestMapper.MapGetUserResultRequestRecommendations(ref app);
// ResultRequestMapper.MapGetRequestRecommendations(ref app);

//Chat
app.MapHub<ChatHub>("/chatHub");
// ML
MLMapper.MapGetGetRecommendedUsers(ref app);
MLMapper.MapGetGetRequestsFrequencyStatistics(ref app);
MLMapper.MapGetGetMostPopularSkills(ref app);
MLMapper.MapGetGetMostPopularHobby(ref app);
MLMapper.MapGetGetMostPopularInterest(ref app);

app.Run();