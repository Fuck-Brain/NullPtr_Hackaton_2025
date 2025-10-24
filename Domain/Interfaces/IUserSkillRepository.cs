using Back.Domain.Entity;

namespace Back.Domain.Interfaces
{
    public interface IUserSkillRepository
    {
        Task<Guid> AddSkillAsync(UserSkill skill);
        Task<IEnumerable<UserSkill>> GetSkillsByUserAsync(Guid userId);
        Task<IEnumerable<UserSkill>> GetAllSkillsAsync();
        Task<IEnumerable<(string SkillName, int Count)>> GetTopSkillsAsync(int top = 10);
        Task DeleteSkillAsync(Guid id);
    }
}
