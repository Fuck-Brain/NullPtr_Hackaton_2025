using Back.Domain.Entity;

namespace Back.Domain.Interfaces
{
    public interface IUserHobbyRepository
    {
        Task<Guid> AddHobbyAsync(UserHobby hobby);
        Task<IEnumerable<UserHobby>> GetHobbiesByUserAsync(Guid userId);
        Task<IEnumerable<UserHobby>> GetAllHobbiesAsync();
        Task<IEnumerable<(string HobbyName, int Count)>> GetTopHobbiesAsync(int top = 10);
        Task DeleteHobbyAsync(Guid id);
    }
}
