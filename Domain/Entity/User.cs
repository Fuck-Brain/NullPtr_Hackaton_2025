namespace Back.Domain.Entity
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Login { get; private set; }
        public string HashPassword { get; private set; }
        public string PhotoHash { get; private set; }
        public string Name { get; private set; }
        public string SurName { get; private set; }
        public string FatherName { get; private set; }
        public int Age { get; private set; }
        public string Gender { get; private set; }
        public string DescribeUser { get; private set; } = string.Empty;
        public string City { get; private set; }
        public string Contact { get; private set; }

        // связи (коллекции)
        public ICollection<UserSkill> Skills { get; private set; } = new List<UserSkill>();
        public ICollection<UserInterest> Interests { get; private set; } = new List<UserInterest>();
        public ICollection<UserGoal> Goals { get; private set; } = new List<UserGoal>();
        public ICollection<UserHobby> Hobbies { get; private set; } = new List<UserHobby>();

        private User() { } // для EF Core

        public User(
            string login,
            string hashPassword,
            string photoHash,
            string name,
            string surName,
            string fatherName,
            int age,
            string gender,
            string city,
            string contact,
            string describeUser = "")
        {
            Id = Guid.NewGuid();
            Login = login;
            HashPassword = hashPassword;
            PhotoHash = photoHash;
            Name = name;
            SurName = surName;
            FatherName = fatherName;
            Age = age;
            Gender = gender;
            City = city;
            Contact = contact;
            DescribeUser = describeUser;
        }

        // методы управления
        public void UpdateDescription(string description) => DescribeUser = description;

        public void AddSkill(string skill) => Skills.Add(new UserSkill(Id, skill));
        public void AddInterest(string interest) => Interests.Add(new UserInterest(Id, interest));
        public void AddGoal(string goal) => Goals.Add(new UserGoal(Id, goal));
        public void AddHobby(string hobby) => Hobbies.Add(new UserHobby(Id, hobby));
    }
}
