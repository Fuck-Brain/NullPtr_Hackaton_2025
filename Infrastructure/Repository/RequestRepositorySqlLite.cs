using Back.Domain.Entity;
using Back.Domain.Interfaces;
using Back.Infrastructure;
using Back.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Back.Infrastructure.Repository
{
    public class RequestRepositorySqlLite : IRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public RequestRepositorySqlLite(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddRequest(Request request)
        {
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();
            return request.Id;
        }

        public async Task DeleteRequest(Guid id)
        {
            var request = await _context.Requests.FirstOrDefaultAsync(x => x.Id == id);
            if (request != null)
            {
                _context.Requests.Remove(request);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Request>> GetAllRequests()
        {
            return await _context.Requests
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<Request?> GetRequest(Guid id)
        {
            return await _context.Requests
                .Include(r => r.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateRequest(Request request)
        {
            _context.Requests.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}
