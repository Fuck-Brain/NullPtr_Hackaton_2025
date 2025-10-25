using Back.Domain.Entity;
using Back.Infrastructure;
using Back.Infrastructure.DataBase;
using Back.Infrastructure.MlClient;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Back.Infrastructure.MLClient
{
    public class MLClient
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;

        public MLClient(HttpClient httpClient, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        // ============================================
        // 1️⃣ Отправка Request -> ML -> обновление в БД
        // ============================================
        public async Task<bool> ProcessRequestAsync(Guid requestId)
        {
            var request = await _context.Requests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
                return false;

            var payload = new
            {
                Id = request.Id,
                UserId = request.UserId,
                NameRequest = request.NameRequest,
                TextRequest = request.TextRequest,
                Label = request.Label,
                IsSended = request.IsSended
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("classifier", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<MLResultResponse>(responseJson);

            // обновляем данные в БД
            if (result != null)
            {
                request.SetLabel(result.Label);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        // ==============================================================
        // 2️⃣ Отправка Request + User + список всех пользователей -> ML
        // ==============================================================

        public async Task<IEnumerable<User>> GetRecommendedUsersAsync(Guid requestId, Guid userId)
        {
            var request = await _context.Requests.FindAsync(requestId);
            var allResuest = await _context.Requests.ToListAsync();
            var allUsers = await _context.Users.Include(x => x.Hobbies).
                Include(x => x.Interests).
                Include(x => x.Skills).
                Where(x => x.Id != userId).
                ToListAsync();

            if (request == null)
                return Enumerable.Empty<User>();

            var payload = new
            {
                Request = new
                {
                    Id =  request.Id,
                    UserId = request.UserId,
                    NameRequest = request.NameRequest,
                    TextRequest = request.TextRequest,  
                    Label = request.Label,
                    IsSended = request.IsSended
                },
                Users = allUsers.Select(u => new
                {
                    Id = u.Id,
                    Name = u.Name,
                    SurName = u.SurName,
                    FatherName = u.FatherName,
                    Age = u.Age,
                    City = u.City,
                    Gender = u.Gender,
                    Describe = u.DescribeUser,
                    Description = u.DescribeUser,
                    UserSkills = string.Join(", ", u.Skills.Select(s => s.SkillName)),
                    UserHobbies = string.Join(", ", u.Hobbies.Select(s => s.HobbyName)),
                    UserInterest = string.Join(", ", u.Interests.Select(s => s.InterestName))
                }).ToList(),
                Requests = allResuest.Select(r => new
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    NameRequest = r.NameRequest,
                    TextRequest = r.TextRequest,
                    Label = r.Label,
                    IsSended = r.IsSended
                })
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("predict", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<Guid>>(responseJson);

            if (result == null || result.Count == 0)
                return Enumerable.Empty<User>();

            // Возвращаем пользователей, которых рекомендовал ML
            return allUsers.Where(u => result.Contains(u.Id)).ToList();
        }

    }
}
