using Back.Domain.Entity;

namespace Back.Domain.Interfaces
{
    public interface IUserInterestRepository
    {
        Task<Guid> AddInterestAsync(UserInterest interest);
        Task<IEnumerable<UserInterest>> GetInterestsByUserAsync(Guid userId);
        Task<IEnumerable<UserInterest>> GetAllInterestsAsync();
        Task<IEnumerable<(string InterestName, int Count)>> GetTopInterestsAsync(int top = 10);
        Task DeleteInterestAsync(Guid id);
    }
}
