namespace Back.Domain.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login {  get; set; }
        public string HashPassword { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string FatherName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string DescribeUser { get; set; } = String.Empty;
        public string Skills { get; set; } = String.Empty;
        public string City { get; set; }
        public string Contact { get; set; }

        public User() { }

        public User(string login, string hashPassword, string name, string surName, string fatherName, int age, string gender, string city, string contact)
        {
            Id = Guid.NewGuid();
            Login = login;
            HashPassword = hashPassword;
            Name = name;
            SurName = surName;
            FatherName = fatherName;
            Age = age;
            Gender = gender;
            City = city;
            Contact = contact;
        }
    }
}
