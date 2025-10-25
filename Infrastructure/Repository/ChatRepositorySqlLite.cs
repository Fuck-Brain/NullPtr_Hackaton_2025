using Back.Domain.Interfaces;
using Back.Domain.Entity;
using Back.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Back.Infrastructure.Repository
{
    public class ChatRepositorySqlLite : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepositorySqlLite(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddMessageAsync(Guid chatId,Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }

        public async Task<Chat> CreateChatAsync(Guid userA, Guid userB)
        {
            User userFirst = await _context.Users.FirstOrDefaultAsync(x => x.Id == userA);
            User userSecond = await _context.Users.FirstOrDefaultAsync(x => x.Id == userB);

            if(userFirst == null || userSecond == null)
            {
                return null;
            }
            Chat chat = new Chat(userA, userB);
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();
            return chat;
        }

        public async Task<Chat?> GetChatAsync(Guid userA, Guid userB)
        {
            Chat findChat = await _context.Chats.Include(x => x.Messages).FirstOrDefaultAsync(x => (x.UserAId == userA && x.UserBId == userB) || (x.UserAId == userB && x.UserBId == userA));
            return findChat;
            
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(Guid chatId)
        {
            Chat findChat = await _context.Chats.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == chatId);
            return findChat.Messages ?? Enumerable.Empty<Message>();
        }
    }
}
