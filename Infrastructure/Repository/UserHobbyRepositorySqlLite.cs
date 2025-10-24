using Back.Domain.Entity;
using Back.Domain.Interfaces;
using Back.Infrastructure;
using Back.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Back.Infrastructure.Repository
{
    public class UserHobbyRepositorySqlLite : IUserHobbyRepository
    {
        private readonly ApplicationDbContext _context;

        public UserHobbyRepositorySqlLite(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddHobbyAsync(UserHobby hobby)
        {
            await _context.UserHobbies.AddAsync(hobby);
            await _context.SaveChangesAsync();
            return hobby.Id;
        }

        public async Task<IEnumerable<UserHobby>> GetHobbiesByUserAsync(Guid userId)
        {
            return await _context.UserHobbies
                .Where(h => h.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserHobby>> GetAllHobbiesAsync()
        {
            return await _context.UserHobbies.ToListAsync();
        }

        public async Task<IEnumerable<(string HobbyName, int Count)>> GetTopHobbiesAsync(int top = 10)
        {
            return await _context.UserHobbies
                .GroupBy(h => h.HobbyName)
                .Select(g => new ValueTuple<string, int>(g.Key, g.Count()))
                .OrderByDescending(x => x.Item2)
                .Take(top)
                .ToListAsync();
        }

        public async Task DeleteHobbyAsync(Guid id)
        {
            var hobby = await _context.UserHobbies.FirstOrDefaultAsync(h => h.Id == id);
            if (hobby != null)
            {
                _context.UserHobbies.Remove(hobby);
                await _context.SaveChangesAsync();
            }
        }
    }
}
