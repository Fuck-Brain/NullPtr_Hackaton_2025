using Back.Domain.Entity;

namespace Back.Application;

public class UserServices
{
    public UserServices()
    {
        
    }

    public string Login(string login, string password, string name, string surName, string fatherName, int age, string gender, string city, string contact)
    {
        // TODO: check if user exists
        var user = new User(login, password, name, surName, fatherName, age, gender, city, contact);
        // TODO: add to repo

        string token = "token";
        return token;
    }

    public string Register(string login, string password)
    {
        // TODO: check is all data correct
        string token = "token";
        return token;
    }

    public void Update(Guid id, UserUpdateDto info)
    {
        // TODO: get user
        
        if (info.Login is not null) { }
        if (info.Password is not null) { }
        if (info.Name is not null) { }
        if (info.Surname is not null) { }
        if (info.FatherName is not null) { }
        if (info.Age is not null) { }
        if (info.Gender is not null) { }
        if (info.City is not null) { }
        if (info.Contact is not null) { }
        if (info.Skills is not null) { }
        if (info.Description is not null) { }
        
        
        // TODO: update user
    }
}