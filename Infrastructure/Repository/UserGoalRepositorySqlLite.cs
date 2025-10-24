using Back.Domain.Entity;
using Back.Domain.Interfaces;
using Back.Infrastructure;
using Back.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Back.Infrastructure.Repository
{
    public class UserGoalRepositorySqlLite : IUserGoalRepository
    {
        private readonly ApplicationDbContext _context;

        public UserGoalRepositorySqlLite(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddGoalAsync(UserGoal goal)
        {
            await _context.UserGoals.AddAsync(goal);
            await _context.SaveChangesAsync();
            return goal.Id;
        }

        public async Task<IEnumerable<UserGoal>> GetGoalsByUserAsync(Guid userId)
        {
            return await _context.UserGoals
                .Where(g => g.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserGoal>> GetAllGoalsAsync()
        {
            return await _context.UserGoals.ToListAsync();
        }

        public async Task<IEnumerable<(string GoalName, int Count)>> GetTopGoalsAsync(int top = 10)
        {
            return await _context.UserGoals
                .GroupBy(g => g.GoalName)
                .Select(g => new ValueTuple<string, int>(g.Key, g.Count()))
                .OrderByDescending(x => x.Item2)
                .Take(top)
                .ToListAsync();
        }

        public async Task DeleteGoalAsync(Guid id)
        {
            var goal = await _context.UserGoals.FirstOrDefaultAsync(g => g.Id == id);
            if (goal != null)
            {
                _context.UserGoals.Remove(goal);
                await _context.SaveChangesAsync();
            }
        }
    }
}
