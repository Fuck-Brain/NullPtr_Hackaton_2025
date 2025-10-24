namespace Back.Domain.Entity
{
    public class UserInterest
    {
        public Guid Id { get;  set; }
        public Guid UserId { get;  set; }
        public string InterestName { get;  set; }

        public User User { get;  set; }

        private UserInterest() { }

        public UserInterest(Guid userId, string interestName)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            InterestName = interestName;
        }
    }
}
