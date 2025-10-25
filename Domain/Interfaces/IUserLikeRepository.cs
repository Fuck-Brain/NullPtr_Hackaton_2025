using Back.Domain.Entity;

namespace Back.Domain.Interfaces
{
    public interface IUserLikeRepository
    {
        Task<Guid> AddAsync(UserLike like);
        Task<UserLike?> GetAsync(Guid fromUserId, Guid toUserId);
        Task<IEnumerable<UserLike>> GetAllAsync();
        Task<IEnumerable<UserLike>> GetToUserAsync(Guid userId);
        Task<IEnumerable<UserLike>> GetUserLikes(Guid userId);
        Task UpdateUserLike(UserLike like);
        Task DeleteAsync(Guid id);
    }
}
