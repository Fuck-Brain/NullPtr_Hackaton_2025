using Back.Domain.Entity;

namespace Back.Domain.Interfaces
{
    public interface IUserRepository
    {
        public Task<Guid> AddUser(User user);
        public Task<User> GetUser(Guid id);
        public Task<IEnumerable<User>> GetAllUser();
        public Task UpdateUser(User user);
        public Task DeleteUser(Guid id);
    }
}
