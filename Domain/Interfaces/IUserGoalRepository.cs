using Back.Domain.Entity;

namespace Back.Domain.Interfaces
{
    public interface IUserGoalRepository
    {
        Task<Guid> AddGoalAsync(UserGoal goal);
        Task<IEnumerable<UserGoal>> GetGoalsByUserAsync(Guid userId);
        Task<IEnumerable<UserGoal>> GetAllGoalsAsync();
        Task<IEnumerable<(string GoalName, int Count)>> GetTopGoalsAsync(int top = 10);
        Task DeleteGoalAsync(Guid id);
    }
}
