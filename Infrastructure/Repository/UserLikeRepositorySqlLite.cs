using Back.Domain.Entity;
using Back.Domain.Interfaces;
using Back.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;


namespace Back.Infrastructure.Repository
{
    public class UserLikeRepositorySqlLite : IUserLikeRepository
    {
        private readonly ApplicationDbContext _context;

        public UserLikeRepositorySqlLite(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddAsync(UserLike like)
        {
            await _context.UserLikes.AddAsync(like);
            await _context.SaveChangesAsync();
            return like.Id;
        }

        public async Task<UserLike?> GetAsync(Guid fromUserId, Guid toUserId)
        {
            return await _context.UserLikes
                .Include(x => x.FromUser)
                .Include(x => x.ToUser)
                .FirstOrDefaultAsync(x => x.FromUserId == fromUserId && x.ToUserId == toUserId);
        }

        public async Task<IEnumerable<UserLike>> GetUserLikes(Guid userId)
        {
            return await _context.UserLikes
                .Where(x => x.FromUserId == userId)
                .Include(x => x.FromUser)
                .Include(x => x.ToUser)
                .ToListAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var like = await _context.UserLikes.FirstOrDefaultAsync(x => x.Id == id);
            if (like != null)
            {
                _context.UserLikes.Remove(like);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UserLike>> GetAllAsync()
        {
            return await _context.UserLikes.ToListAsync();
        }

        public async Task UpdateUserLike(UserLike like)
        {
            _context.Update(like);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserLike>> GetToUserAsync(Guid userId)
        {
            return await _context.UserLikes
                .Where(x => x.ToUserId == userId)
                .Include(x => x.FromUser)
                .Include(x => x.ToUser)
                .ToListAsync();
        }
    }
}
