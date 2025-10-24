using Back.Domain.Entity;
using Back.Domain.Interfaces;
using Back.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Back.Infrastructure.Repository
{
    public class ResultRequestRepositorySqlLite : IResultRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public ResultRequestRepositorySqlLite(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AddResultRequest(ResultRequest resultRequest)
        {
            await _context.ResultRequests.AddAsync(resultRequest);
            await _context.SaveChangesAsync();
            return resultRequest.Id;
        }

        public async Task<ResultRequest?> GetResultRequest(Guid id)
        {
            return await _context.ResultRequests
                .Include(r => r.Request)
                .Include(r => r.UserRequest)
                .Include(r => r.ResultRequestUsers)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ResultRequest>> GetAllResultRequests()
        {
            return await _context.ResultRequests
                .Include(r => r.Request)
                .Include(r => r.UserRequest)
                .Include(r => r.ResultRequestUsers)
                .ToListAsync();
        }

        public async Task UpdateResultRequest(ResultRequest updated)
        {
            var existing = await _context.ResultRequests
                .Include(r => r.ResultRequestUsers)
                .FirstOrDefaultAsync(r => r.Id == updated.Id);

            if (existing == null)
                return;

            // Обновляем простые поля
            existing.RequestId = updated.RequestId;
            existing.UserRequestId = updated.UserRequestId;

            // Синхронизируем many-to-many
            existing.ResultRequestUsers.Clear();
            foreach (var u in updated.ResultRequestUsers)
            {
                // Attach существующих пользователей
                _context.Attach(u);
                existing.ResultRequestUsers.Add(u);
            }

            await _context.SaveChangesAsync();
        }


        public async Task DeleteResultRequest(Guid id)
        {
            var entity = await _context.ResultRequests.FirstOrDefaultAsync(x => x.Id == id);
            if (entity != null)
            {
                _context.ResultRequests.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
