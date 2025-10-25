using System;

namespace Back.Domain.Entity
{
    public class Message
    {
        public Guid Id { get; private set; }
        public Guid ChatId { get; private set; }
        public Guid SenderId { get; private set; }
        public string Content { get; private set; }
        public DateTime SentAt { get; private set; } = DateTime.UtcNow;

        public Chat Chat { get; private set; }

        private Message() { }

        public Message(Guid chatId, Guid senderId, string content)
        {
            Id = Guid.NewGuid();
            ChatId = chatId;
            SenderId = senderId;
            Content = content;
        }
    }
}
