using Back.Domain.Entity;

namespace Back.Domain.Interfaces
{
    public interface IUserRepository
    {
        public Guid AddUser(User user);
        public User GetUser(Guid id);
        public IEnumerable<User> GetAllUser();
        public void UpdateUser(User user);
        public void DeleteUser(Guid id);
    }
}
