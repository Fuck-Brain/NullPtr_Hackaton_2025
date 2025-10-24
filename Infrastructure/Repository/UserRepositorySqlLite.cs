using Back.Domain.Entity;
using Back.Domain.Interfaces;
using Back.Infrastructure;
using Back.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Back.Infrastructure.Repository
{
    public class UserRepositorySqlLite : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepositorySqlLite(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task DeleteUser(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetAllUser()
        {
            return await _context.Users
                .ToListAsync();
        }

        public async Task<User?> GetUser(Guid id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
