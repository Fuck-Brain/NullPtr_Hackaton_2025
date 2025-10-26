using Back.Domain.Entity;
using Back.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Back.Infrastructure.MLClient // ← единый неймспейс
{
    public class MLClient
    {
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };

        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;

        public MLClient(HttpClient httpClient, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task<bool> ProcessRequestAsync(Guid requestId, CancellationToken ct = default)
        {
            var request = await _context.Requests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == requestId, ct);

            if (request == null) return false;

            var payload = new
            {
                request.UserId,
                request.NameRequest,
                request.TextRequest,
                request.Label
            };

            var content = new StringContent(JsonSerializer.Serialize(payload, JsonOpts), Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync("classifier", content, ct);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(ct);
            var label = responseJson.Trim('"', ' ', '\n', '\r', '\t');

            if (string.IsNullOrWhiteSpace(label)) return false;

            request.SetLabel(label);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<IEnumerable<User>> GetRecommendedUsersAsync(Guid requestId, Guid userId, CancellationToken ct = default)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(r => r.Id == requestId, ct);
            var allRequests = await _context.Requests.ToListAsync();
            User currentUser = await _context.Users.Include(x => x.Hobbies)
                .Include(x => x.Interests)
                .Include(x => x.Skills)
                .FirstOrDefaultAsync(x => x.Id == userId);
            var allUsers = await _context.Users.Include(x => x.Hobbies)
                .Include(x => x.Interests)
                .Include(x => x.Skills)
                .ToListAsync(ct);

            if (request == null) return Enumerable.Empty<User>();

            var usersForPayload = allUsers.Select(u => new
            {
                u.Id,
                u.DescribeUser,
                Skills    = string.Join(", ", (u.Skills    ?? Enumerable.Empty<UserSkill>()).Select(s => s.SkillName)),
                Interests = string.Join(", ", (u.Interests ?? Enumerable.Empty<UserInterest>()).Select(s => s.InterestName)),
                Hobbies   = string.Join(", ", (u.Hobbies   ?? Enumerable.Empty<UserHobby>()).Select(s => s.HobbyName))
            }).ToList();

            var payload = new
            {
                Request = new
                {
                    request.UserId,
                    request.NameRequest,
                    request.TextRequest,
                    request.Label
                },
                Users = usersForPayload,
                Requests = allRequests.Select(r => new
                {
                    r.UserId,
                    r.NameRequest,
                    r.TextRequest,
                    r.Label
                }).ToList()
            };

            var content = new StringContent(JsonSerializer.Serialize(payload, JsonOpts), Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync("predict", content, ct);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(ct);

            // Поддержим несколько форматов ответа:
            // 1) { "<guid>": score, ... }
            // 2) [ { "id":"<guid>", "score": <double> }, ... ]
            // 3) [ "<guid1>", "<guid2>", ... ]
            Dictionary<Guid, double> scores = new();

            if (responseJson.TrimStart().StartsWith("{"))
            {
                // Объект со строковыми ключами → GUID
                var asStringDict = JsonSerializer.Deserialize<Dictionary<string, double>>(responseJson, JsonOpts);
                if (asStringDict != null)
                {
                    foreach (var (k, v) in asStringDict)
                        if (Guid.TryParse(k, out var g)) scores[g] = v;
                }
            }
            else if (responseJson.TrimStart().StartsWith("["))
            {
                // Попытка 1: массив объектов { id, score }
                try
                {
                    var arr = JsonSerializer.Deserialize<List<RecItem>>(responseJson, JsonOpts);
                    if (arr != null && arr.Count > 0 && arr.Any(x => x.Id != Guid.Empty))
                    {
                        foreach (var item in arr)
                            if (item.Id != Guid.Empty) scores[item.Id] = item.Score;
                    }
                    else
                    {
                        // Попытка 2: массив строк-гайдов
                        var ids = JsonSerializer.Deserialize<List<string>>(responseJson, JsonOpts);
                        if (ids != null)
                        {
                            foreach (var s in ids)
                                if (Guid.TryParse(s, out var g)) scores[g] = 1.0; // равные веса, если не пришли
                        }
                    }
                }
                catch
                {
                    // Fallback на массив строк
                    var ids = JsonSerializer.Deserialize<List<string>>(responseJson, JsonOpts);
                    if (ids != null)
                        foreach (var s in ids)
                            if (Guid.TryParse(s, out var g)) scores[g] = 1.0;
                }
            }

            if (scores.Count == 0) return Enumerable.Empty<User>();

            // Вернём пользователей в порядке убывания score
            var usersById = allUsers.ToDictionary(u => u.Id, u => u);
            var ordered = scores
                .OrderByDescending(kv => kv.Value)
                .Select(kv => usersById.TryGetValue(kv.Key, out var u) ? u : null)
                .Where(u => u != null)!;

            return ordered!;
        }

        public async Task<Dictionary<string, int>> GetRequestsFrequencyStatisticsAsync(FilterOptions? filter = null, CancellationToken ct = default)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter?.City))
                query = query.Where(u => u.City != null && u.City.ToLower() == filter.City!.ToLower());

            if (!string.IsNullOrWhiteSpace(filter?.Gender))
                query = query.Where(u => u.Gender != null && u.Gender.ToLower() == filter.Gender!.ToLower());

            if (filter?.MinAge != null)
                query = query.Where(u => u.Age >= filter.MinAge);

            if (filter?.MaxAge != null)
                query = query.Where(u => u.Age <= filter.MaxAge);

            var filteredUserIds = await query.Select(u => u.Id).ToListAsync(ct);

            var allSkills = await _context.UserSkills
                .Where(s => filteredUserIds.Contains(s.UserId))
                .Select(s => s.SkillName)
                .Distinct()
                .ToListAsync(ct);

            var allRequests = await _context.Requests
                .Where(r => filteredUserIds.Contains(r.UserId))
                .Select(r => new { r.UserId, r.NameRequest, r.TextRequest, r.Label })
                .ToListAsync(ct);

            var payload = new { Skills = allSkills, Requests = allRequests };
            var content = new StringContent(JsonSerializer.Serialize(payload, JsonOpts), Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync("statistic/requests_frequency", content, ct);
            response.EnsureSuccessStatusCode();

            var resJson = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(resJson, JsonOpts) ?? new();
        }

        public async Task<Dictionary<string, int>> GetMostPopularSkillsAsync(FilterOptions? filter = null, CancellationToken ct = default)
        {
            var users = _context.Users.Include(u => u.Skills).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter?.City))
                users = users.Where(u => u.City != null && u.City.ToLower() == filter.City!.ToLower());
            if (!string.IsNullOrWhiteSpace(filter?.Gender))
                users = users.Where(u => u.Gender != null && u.Gender.ToLower() == filter.Gender!.ToLower());
            if (filter?.MinAge != null)
                users = users.Where(u => u.Age >= filter.MinAge);
            if (filter?.MaxAge != null)
                users = users.Where(u => u.Age <= filter.MaxAge);

            var list = await users.ToListAsync(ct);

            var profiles = list.Select(u => string.Join(", ", (u.Skills ?? Enumerable.Empty<UserSkill>()).Select(s => s.SkillName))).ToArray();

            var content = new StringContent(JsonSerializer.Serialize(profiles, JsonOpts), Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync("statistic/most_popular", content, ct);
            response.EnsureSuccessStatusCode();

            var resJson = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(resJson, JsonOpts) ?? new();
        }

        public async Task<Dictionary<string, int>> GetMostPopularHobbyAsync(FilterOptions? filter = null, CancellationToken ct = default)
        {
            var users = _context.Users.Include(u => u.Hobbies).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter?.City))
                users = users.Where(u => u.City != null && u.City.ToLower() == filter.City!.ToLower());
            if (!string.IsNullOrWhiteSpace(filter?.Gender))
                users = users.Where(u => u.Gender != null && u.Gender.ToLower() == filter.Gender!.ToLower());
            if (filter?.MinAge != null)
                users = users.Where(u => u.Age >= filter.MinAge);
            if (filter?.MaxAge != null)
                users = users.Where(u => u.Age <= filter.MaxAge);

            var list = await users.ToListAsync(ct);

            var profiles = list.Select(u => string.Join(", ", (u.Hobbies ?? Enumerable.Empty<UserHobby>()).Select(s => s.HobbyName))).ToArray();

            var content = new StringContent(JsonSerializer.Serialize(profiles, JsonOpts), Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync("statistic/most_popular", content, ct);
            response.EnsureSuccessStatusCode();

            var resJson = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(resJson, JsonOpts) ?? new();
        }

        public async Task<Dictionary<string, int>> GetMostPopularInterestAsync(FilterOptions? filter = null, CancellationToken ct = default)
        {
            var users = _context.Users.Include(u => u.Interests).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter?.City))
                users = users.Where(u => u.City != null && u.City.ToLower() == filter.City!.ToLower());
            if (!string.IsNullOrWhiteSpace(filter?.Gender))
                users = users.Where(u => u.Gender != null && u.Gender.ToLower() == filter.Gender!.ToLower());
            if (filter?.MinAge != null)
                users = users.Where(u => u.Age >= filter.MinAge);
            if (filter?.MaxAge != null)
                users = users.Where(u => u.Age <= filter.MaxAge);

            var list = await users.ToListAsync(ct);

            var profiles = list.Select(u => string.Join(", ", (u.Interests ?? Enumerable.Empty<UserInterest>()).Select(s => s.InterestName))).ToArray();

            var content = new StringContent(JsonSerializer.Serialize(profiles, JsonOpts), Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync("statistic/most_popular", content, ct);
            response.EnsureSuccessStatusCode();

            var resJson = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(resJson, JsonOpts) ?? new();
        }

        private record RecItem(Guid Id, double Score);
    }
}
