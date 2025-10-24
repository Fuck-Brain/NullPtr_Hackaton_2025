namespace Back.Domain.Entity
{
    public class UserSkill
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string SkillName { get; private set; }

        public User User { get; private set; }

        private UserSkill() { }

        public UserSkill(Guid userId, string skillName)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            SkillName = skillName;
        }
    }
}
