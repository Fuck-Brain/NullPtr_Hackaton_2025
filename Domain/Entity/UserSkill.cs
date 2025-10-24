namespace Back.Domain.Entity
{
    public class UserSkill
    {
        public Guid Id { get;  set; }
        public Guid UserId { get;  set; }
        public string SkillName { get;  set; }

        public User User { get;  set; }

        private UserSkill() { }

        public UserSkill(Guid userId, string skillName)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            SkillName = skillName;
        }
    }
}
