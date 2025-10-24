namespace Back.Domain.Entity
{
    public class User
    {
        public Guid Id { get;  set; }
        public string Login { get;  set; }
        public string HashPassword { get;  set; }
        public string PhotoHash { get;  set; }
        public string Name { get;  set; }
        public string SurName { get;  set; }
        public string FatherName { get;  set; }
        public int Age { get;  set; }
        public string Gender { get;  set; }
        public string DescribeUser { get;  set; } = string.Empty;
        public string City { get;  set; }
        public string Contact { get;  set; }

        // связи (коллекции)
        public ICollection<UserSkill> Skills { get;  set; } = new List<UserSkill>();
        public ICollection<UserInterest> Interests { get;  set; } = new List<UserInterest>();
        public ICollection<Request> Requests { get;  set; } = new List<Request>();
        public ICollection<UserHobby> Hobbies { get;  set; } = new List<UserHobby>();

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
        public void AddRequests(string name,string goal) => Requests.Add(new Request(Id,name, goal));
        public void AddHobby(string hobby) => Hobbies.Add(new UserHobby(Id, hobby));
    }
}
