namespace Back.Domain.Entity
{
    public class UserLike
    {
        public Guid Id { get; private set; }
        public Guid FromUserId { get; private set; }
        public Guid ToUserId { get; private set; }
        public bool IsLike { get; private set; }

        public User FromUser { get; private set; }
        public User ToUser { get; private set; }

        private UserLike() { }

        public UserLike(Guid fromUserId, Guid toUserId, bool isLike)
        {
            Id = Guid.NewGuid();
            FromUserId = fromUserId;
            ToUserId = toUserId;
            IsLike = isLike;
        }

        public void ChangeReaction(bool isLike)
        {
            IsLike = isLike;
        }
    }
}
