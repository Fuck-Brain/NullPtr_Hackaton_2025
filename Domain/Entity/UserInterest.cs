namespace Back.Domain.Entity
{
    public class UserInterest
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string InterestName { get; private set; }

        public User User { get; private set; }

        private UserInterest() { }

        public UserInterest(Guid userId, string interestName)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            InterestName = interestName;
        }
    }
}
