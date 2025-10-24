namespace Back.Domain.Entity
{
    public class UserHobby
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string HobbyName { get; private set; }

        public User User { get; private set; }

        private UserHobby() { }

        public UserHobby(Guid userId, string hobbyName)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            HobbyName = hobbyName;
        }
    }
}
