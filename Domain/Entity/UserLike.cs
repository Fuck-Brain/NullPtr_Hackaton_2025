namespace Back.Domain.Entity
{
    public class UserLike
    {
        public Guid Id { get;  set; }
        public Guid FromUserId { get;  set; }
        public Guid ToUserId { get;  set; }
        public bool IsLike { get; set; }

        public User FromUser { get;  set; }
        public User ToUser { get;  set; }

        private UserLike() { }

        public UserLike(Guid fromUserId, Guid toUserId)
        {
            Id = Guid.NewGuid();
            FromUserId = fromUserId;
            ToUserId = toUserId;
        }

        public void ChangeReaction(bool isLike)
        {
            IsLike = isLike;
        }
    }
}
