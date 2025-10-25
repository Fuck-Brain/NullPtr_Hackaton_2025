namespace Back.Domain.Entity
{
    public class Chat
    {
        public Guid Id { get; private set; }
        public Guid UserAId { get; private set; }
        public Guid UserBId { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; private set; } = new List<Message>();

        private Chat() { }

        public Chat(Guid userAId, Guid userBId)
        {
            Id = Guid.NewGuid();
            UserAId = userAId;
            UserBId = userBId;
        }
    }
}
