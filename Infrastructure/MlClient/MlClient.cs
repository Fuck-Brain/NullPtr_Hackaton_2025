using Back.Domain.Entity;
using Back.Infrastructure;
using Back.Infrastructure.DataBase;
using Back.Infrastructure.MlClient;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                UserId = request.UserId,
                NameRequest = request.NameRequest,
                TextRequest = request.TextRequest,
                Label = request.Label
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("classifier", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            // сервер возвращает просто строку, а не объект
            var label = responseJson.Trim('"');

            if (!string.IsNullOrWhiteSpace(label))
            {
                request.SetLabel(label);
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
            User currentUser = await _context.Users.Include(x => x.Hobbies)
                .Include(x => x.Interests)
                .Include(x => x.Skills)
                .FirstOrDefaultAsync(x => x.Id == userId);
            var allUsers = await _context.Users.Include(x => x.Hobbies)
                .Include(x => x.Interests)
                .Include(x => x.Skills)
                .Where(x => x.Id != userId)
                .ToListAsync();

            if (request == null)
                return Enumerable.Empty<User>();

            var payload = new
            {
                Request = new
                {
                    UserId = request.UserId,
                    NameRequest = request.NameRequest,
                    TextRequest = request.TextRequest,
                    Label = request.Label
                },
                Users = allUsers.Select(u => new
                {
                    Id = u.Id,
                    DescribeUser = u.DescribeUser,
                    Skills = string.Join(", ", u.Skills.Select(s => s.SkillName)),
                    Interests = string.Join(", ", u.Interests.Select(s => s.InterestName)),
                    Hobbies = string.Join(", ", u.Hobbies.Select(s => s.HobbyName))
                }).ToList(),
                Requests = allResuest.Select(r => new
                {
                    UserId = r.UserId,
                    NameRequest = r.NameRequest,
                    TextRequest = r.TextRequest,
                    Label = r.Label
                }).ToList()
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("predict", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<Guid, double>>(responseJson);

            if (result == null || result.Count == 0)
                return Enumerable.Empty<User>();

            return allUsers.Where(u => result.ContainsKey(u.Id)).ToList();
        }

        public async Task<Dictionary<string, int>> GetRequestsFrequencyStatisticsAsync(FilterOptions? filter = null)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter?.City))
                query = query.Where(u => u.City.ToLower() == filter.City.ToLower());

            if (!string.IsNullOrWhiteSpace(filter?.Gender))
                query = query.Where(u => u.Gender.ToLower() == filter.Gender.ToLower());

            if (filter?.MinAge != null)
                query = query.Where(u => u.Age >= filter.MinAge);

            if (filter?.MaxAge != null)
                query = query.Where(u => u.Age <= filter.MaxAge);

            var filteredUserIds = await query.Select(u => u.Id).ToListAsync();

            List<string> allSkills = await _context.UserSkills
                .Where(s => filteredUserIds.Contains(s.UserId))
                .Select(s => s.SkillName)
                .Distinct()
                .ToListAsync();

            var allRequests = await _context.Requests
                .Where(r => filteredUserIds.Contains(r.UserId))
                .Select(r => new
                {
                    UserId = r.UserId,
                    NameRequest = r.NameRequest,
                    TextRequest = r.TextRequest,
                    Label = r.Label
                })
                .ToListAsync();

            var payload = new
            {
                Skills = allSkills, // исправлено: теперь это список, а не строка
                Requests = allRequests
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("statistic/requests_frequency", content);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"ML server returned {response.StatusCode} for requests_frequency");

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, int>>(responseJson);

            return result ?? new Dictionary<string, int>();
        }

        public async Task<Dictionary<string, int>> GetMostPopularSkillsAsync(FilterOptions? filter = null)
        {
            var users = _context.Users
                .Include(u => u.Skills)
                .AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter?.City))
                    users = users.Where(u => u.City.ToLower() == filter.City.ToLower());

                if (!string.IsNullOrWhiteSpace(filter?.Gender))
                    users = users.Where(u => u.Gender.ToLower() == filter.Gender.ToLower());

                if (filter?.MinAge != null)
                    users = users.Where(u => u.Age >= filter.MinAge);

                if (filter?.MaxAge != null)
                    users = users.Where(u => u.Age <= filter.MaxAge);
            }

            var filteredUserIds = await users.ToListAsync();

            var userProfiles = filteredUserIds.Select(u =>
                string.Join(", ", u.Skills.Select(s => s.SkillName))
            ).ToArray();

            // исправлено: отправляем массив, а не объект
            var json = JsonSerializer.Serialize(userProfiles);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("statistic/most_popular", content);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"ML server returned {response.StatusCode} for most_popular");

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, int>>(responseJson);

            return result ?? new Dictionary<string, int>();
        }

        public async Task<Dictionary<string, int>> GetMostPopularHobbyAsync(FilterOptions? filter = null)
        {
            var users = _context.Users
                .Include(u => u.Hobbies)
                .AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter?.City))
                    users = users.Where(u => u.City.ToLower() == filter.City.ToLower());

                if (!string.IsNullOrWhiteSpace(filter?.Gender))
                    users = users.Where(u => u.Gender.ToLower() == filter.Gender.ToLower());

                if (filter?.MinAge != null)
                    users = users.Where(u => u.Age >= filter.MinAge);

                if (filter?.MaxAge != null)
                    users = users.Where(u => u.Age <= filter.MaxAge);
            }

            var filteredUserIds = await users.ToListAsync();

            var userProfiles = filteredUserIds.Select(u =>
                string.Join(", ", u.Hobbies.Select(s => s.HobbyName))
            ).ToArray();

            var json = JsonSerializer.Serialize(userProfiles);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("statistic/most_popular", content);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"ML server returned {response.StatusCode} for most_popular");

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, int>>(responseJson);

            return result ?? new Dictionary<string, int>();
        }

        public async Task<Dictionary<string, int>> GetMostPopularInterestAsync(FilterOptions? filter = null)
        {
            var users = _context.Users
                .Include(u => u.Interests)
                .AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter?.City))
                    users = users.Where(u => u.City.ToLower() == filter.City.ToLower());

                if (!string.IsNullOrWhiteSpace(filter?.Gender))
                    users = users.Where(u => u.Gender.ToLower() == filter.Gender.ToLower());

                if (filter?.MinAge != null)
                    users = users.Where(u => u.Age >= filter.MinAge);

                if (filter?.MaxAge != null)
                    users = users.Where(u => u.Age <= filter.MaxAge);
            }

            var filteredUserIds = await users.ToListAsync();

            var userProfiles = filteredUserIds.Select(u =>
                string.Join(", ", u.Interests.Select(s => s.InterestName))
            ).ToArray();

            var json = JsonSerializer.Serialize(userProfiles);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("statistic/most_popular", content);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"ML server returned {response.StatusCode} for most_popular");

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, int>>(responseJson);

            return result ?? new Dictionary<string, int>();
        }
    }
}
