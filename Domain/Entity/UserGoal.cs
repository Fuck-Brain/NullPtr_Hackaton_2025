namespace Back.Domain.Entity
{
    public class UserGoal
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string GoalName { get; private set; }

        public User User { get; private set; }

        private UserGoal() { }

        public UserGoal(Guid userId, string goalName)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            GoalName = goalName;
        }
    }
}
