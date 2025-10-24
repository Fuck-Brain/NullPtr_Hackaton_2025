using Back.Domain.Entity;
using Back.Domain.Interfaces;
using Back.Infrastructure;
using Back.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Back.Infrastructure.Repository
{
    public class UserSkillRepositorySqlLite : IUserSkillRepository
    {
        private readonly ApplicationDbContext _context;

        public UserSkillRepositorySqlLite(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddSkillAsync(UserSkill skill)
        {
            await _context.UserSkills.AddAsync(skill);
            await _context.SaveChangesAsync();
            return skill.Id;
        }

        public async Task<IEnumerable<UserSkill>> GetSkillsByUserAsync(Guid userId)
        {
            return await _context.UserSkills
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserSkill>> GetAllSkillsAsync()
        {
            return await _context.UserSkills.ToListAsync();
        }

        public async Task<IEnumerable<(string SkillName, int Count)>> GetTopSkillsAsync(int top = 10)
        {
            return await _context.UserSkills
                .GroupBy(s => s.SkillName)
                .Select(g => new ValueTuple<string, int>(g.Key, g.Count()))
                .OrderByDescending(x => x.Item2)
                .Take(top)
                .ToListAsync();
        }

        public async Task DeleteSkillAsync(Guid id)
        {
            var skill = await _context.UserSkills.FirstOrDefaultAsync(s => s.Id == id);
            if (skill != null)
            {
                _context.UserSkills.Remove(skill);
                await _context.SaveChangesAsync();
            }
        }
    }
}
