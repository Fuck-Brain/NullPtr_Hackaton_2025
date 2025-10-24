using Back.Domain.Entity;
using Back.Domain.Interfaces;
using Back.Infrastructure;
using Back.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Back.Infrastructure.Repository
{
    public class UserInterestRepositorySqlLite : IUserInterestRepository
    {
        private readonly ApplicationDbContext _context;

        public UserInterestRepositorySqlLite(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddInterestAsync(UserInterest interest)
        {
            await _context.UserInterests.AddAsync(interest);
            await _context.SaveChangesAsync();
            return interest.Id;
        }

        public async Task<IEnumerable<UserInterest>> GetInterestsByUserAsync(Guid userId)
        {
            return await _context.UserInterests
                .Where(i => i.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserInterest>> GetAllInterestsAsync()
        {
            return await _context.UserInterests.ToListAsync();
        }

        public async Task<IEnumerable<(string InterestName, int Count)>> GetTopInterestsAsync(int top = 10)
        {
            return await _context.UserInterests
                .GroupBy(i => i.InterestName)
                .Select(g => new ValueTuple<string, int>(g.Key, g.Count()))
                .OrderByDescending(x => x.Item2)
                .Take(top)
                .ToListAsync();
        }

        public async Task DeleteInterestAsync(Guid id)
        {
            var interest = await _context.UserInterests.FirstOrDefaultAsync(i => i.Id == id);
            if (interest != null)
            {
                _context.UserInterests.Remove(interest);
                await _context.SaveChangesAsync();
            }
        }
    }
}
