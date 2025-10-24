using System.Security.Cryptography;
using System.Text;
using Back.Application.Exceptions;
using Back.Domain.Entity;
using Back.Domain.Interfaces;

namespace Back.Application;

public class UserServices
{
    private readonly IUserRepository _userRepository;
    public UserServices(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string> Login(string login, string password)
    {
        var user = (await _userRepository.GetAllUser()).FirstOrDefault(u => u.Login == login);
        if (user == null)
            throw new AuthException();
        string passwordHash;
        using (var sha = new System.Security.Cryptography.SHA256Managed())
        {
            byte[] textData = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] hash = sha.ComputeHash(textData);
            passwordHash = BitConverter.ToString(hash).Replace("-", String.Empty);
        }

        if (user.HashPassword != passwordHash)
            throw new AuthException();
        
        
        // TODO: gen token
        string token = "token";
        return token;
    }

    public async Task<string> Register(string login, string password, string photoHash, string name, string surName, string fatherName, int age, string gender, string city, string contact)
    {
        if ((await _userRepository.GetAllUser()).Any(u => u.Login == login))
            throw new AuthException();
        
        var user = new User(login, password, photoHash, name, surName, fatherName, age, gender, city, contact);
        
        // TODO: gen token
        string token = "token";
        return token;
    }

    public async Task Update(Guid id, UserUpdateDto info)
    {
        var user = await _userRepository.GetUser(id);
        
        if (info.Login is not null) { user.Login = info.Login; }

        if (info.Password is not null)
        {
            string passwordHash;
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(info.Password);
                byte[] hash = sha.ComputeHash(textData);
                passwordHash = BitConverter.ToString(hash).Replace("-", String.Empty);
            }
            user.HashPassword = passwordHash;
        }
        if (info.Name is not null) { user.Name = info.Name; }
        if (info.Surname is not null) { user.SurName = info.Surname; }
        if (info.FatherName is not null) { user.FatherName = info.FatherName; }
        if (info.Age is not null) { user.Age = (int)info.Age; }
        if (info.Gender is not null) { user.Gender = info.Gender; }
        if (info.City is not null) { user.City = info.City; }
        if (info.Contact is not null) { user.Contact = info.Contact; }
        if (info.Skills is not null) { user.Skills = info.Skills; }
        if (info.Description is not null) { user.DescribeUser = info.Description; }
        
        await _userRepository.UpdateUser(user);
    }
}