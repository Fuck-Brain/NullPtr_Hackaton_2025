using Back.Domain.Entity;

namespace Back.Domain.Interfaces
{
    public interface IChatRepository
    {
        Task<Chat?> GetChatAsync(Guid userA, Guid userB);
        Task<Chat> CreateChatAsync(Guid userA, Guid userB);
        Task AddMessageAsync(Guid chatId, Message message);
        Task<IEnumerable<Message>> GetMessagesAsync(Guid chatId);
    }
}
