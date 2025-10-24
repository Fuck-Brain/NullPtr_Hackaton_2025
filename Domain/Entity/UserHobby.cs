namespace Back.Domain.Entity
{
    public class UserHobby
    {
        public Guid Id { get;  set; }
        public Guid UserId { get;  set; }
        public string HobbyName { get;  set; }

        public User User { get;  set; }

        private UserHobby() { }

        public UserHobby(Guid userId, string hobbyName)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            HobbyName = hobbyName;
        }
    }
}
