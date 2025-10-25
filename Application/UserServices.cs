using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Back.API.DTO;
using Back.Application.Auth;
using Back.Application.Dtos;
using Back.Application.Exceptions;
using Back.Domain.Entity;
using Back.Domain.Interfaces;
using Back.Infrastructure;

namespace Back.Application;

public class UserServices
{
    private readonly IUserRepository _userRepository;
    private readonly UnitOfWork _unit;
    private readonly IJwtTokenService _jwtTokenService;
    public UserServices(IUserRepository userRepository, UnitOfWork unit, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _unit = unit;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> Login(string login, string password)
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

        return new AuthResponseDto() { 
            UserId = user.Id,
            Token = _jwtTokenService.GenerateToken(user.Login)
        };
    }

    public async Task<AuthResponseDto> Register(string login, string password, string photoHash, string name, string surName, string fatherName, int age, string gender, string city, string contact)
    {
        if ((await _userRepository.GetAllUser()).Any(u => u.Login == login))
            throw new AuthException();

        string passwordHash;
        using (var sha = new SHA256Managed())
        {
            byte[] textData = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha.ComputeHash(textData);
            passwordHash = BitConverter.ToString(hash).Replace("-", String.Empty);
        }

        var user = new User(login, passwordHash, photoHash, name, surName, fatherName, age, gender, city, contact);
        await _userRepository.AddUser(user);
        return new AuthResponseDto() { 
            UserId = user.Id,
            Token = _jwtTokenService.GenerateToken(user.Login)
        };
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
        if (info.Skills is not null)
        {
            user.Skills.Clear();
            foreach (var skill in info.Skills)
                user.AddSkill(skill);
        }
        if (info.Interests is not null)
        {
            user.Interests.Clear();
            foreach (var interests in info.Interests)
            {
                user.AddInterest(interests);
            }
        }
        if (info.Hobbies is not null)
        {
            user.Hobbies.Clear();
            foreach (var hobby in info.Hobbies)
                user.AddHobby(hobby);
        }
        if (info.Description is not null) { user.DescribeUser = info.Description; }
        
        await _userRepository.UpdateUser(user);
    }

    public async Task<UserDetailsDto> Me(Guid id) // only for authorized users!!
    {
        var user = await _userRepository.GetUser(id);

        if (user == null)
            throw new AuthException();

        return new UserDetailsDto
        {
            Id = user.Id,
            Login = user.Login,
            HashPassword = user.HashPassword,
            PhotoHash = user.PhotoHash,
            Name = user.Name,
            SurName = user.SurName,
            FatherName = user.FatherName,
            Age = user.Age,
            Gender = user.Gender,
            DescribeUser = user.DescribeUser,
            City = user.City,
            Contact = user.Contact
        };
    }

    public async Task<UserBasicDto> GetUser(Guid id) // for getting other users 
    {
       var usr =  await _userRepository.GetUser(id);
       
       return new UserBasicDto()
       {
           Age = usr.Age,
           Gender = usr.Gender,
           City = usr.City,
           DescribeUser = usr.DescribeUser,
           Name = usr.Name,
           FatherName = usr.FatherName,
           Hobbies = usr.Hobbies,
           Login = usr.Login,
           Id = usr.Id,
           Interests = usr.Interests,
           PhotoHash = usr.PhotoHash,
           Skills = usr.Skills,
           SurName = usr.SurName
       };

    }

    public async Task LikeUser(Guid from, Guid to)
    {
        var like = (await _unit.userLikeRepository.GetUserLikes(from)).FirstOrDefault(l => l.ToUserId == to);

        if (like != null)
            await _unit.userLikeRepository.DeleteAsync(like.Id);

        like = new UserLike(from, to);
        await _unit.userLikeRepository.AddAsync(like);
    }

    public async Task UnLikeUser(Guid from, Guid to)
    {
        var like = (await _unit.userLikeRepository.GetUserLikes(from)).FirstOrDefault(l => l.ToUserId == to && l.FromUserId == from);
        if (like == null)
            throw new ArgumentException("UserLike not found");
        
        await _unit.userLikeRepository.DeleteAsync(like.Id);
    }

    public async Task<List<UserBasicDto>> GetLiked(Guid id)
    {
        return (await _unit.userLikeRepository.GetUserLikes(id)).Select(l =>
        {
            var usr = l.ToUser;
            return new UserBasicDto()
            {
                Age = usr.Age,
                Gender = usr.Gender,
                City = usr.City,
                DescribeUser = usr.DescribeUser,
                Name = usr.Name,
                FatherName = usr.FatherName,
                Hobbies = usr.Hobbies,
                Login = usr.Login,
                Id = usr.Id,
                Interests = usr.Interests,
                PhotoHash = usr.PhotoHash,
                Skills = usr.Skills,
                SurName = usr.SurName
            };
        }).ToList();
    }

    public async Task<List<UserBasicDto>> GetHasLiked(Guid id)
    {
        return (await _unit.userLikeRepository.GetToUserAsync(id)).Select(l =>
        {
            var usr = l.ToUser;
            return new UserBasicDto()
            {
                Age = usr.Age,
                Gender = usr.Gender,
                City = usr.City,
                DescribeUser = usr.DescribeUser,
                Name = usr.Name,
                FatherName = usr.FatherName,
                Hobbies = usr.Hobbies,
                Login = usr.Login,
                Id = usr.Id,
                Interests = usr.Interests,
                PhotoHash = usr.PhotoHash,
                Skills = usr.Skills,
                SurName = usr.SurName
            };
        }).ToList();
    }

    public async Task<List<UserMatchDto>> GetMatches(Guid id)
    {
        var toUser = await _unit.userLikeRepository.GetToUserAsync(id);
        var fromUser = await _unit.userLikeRepository.GetUserLikes(id);
        
        return toUser
            .Where(to => fromUser.Any(fr =>
                fr.FromUserId == to.ToUserId && fr.ToUserId == to.FromUserId))
            .Select(l =>
            {
                var usr = l.ToUser;
                return new UserMatchDto()
                {
                    Age = usr.Age,
                    Gender = usr.Gender,
                    City = usr.City,
                    DescribeUser = usr.DescribeUser,
                    Name = usr.Name,
                    FatherName = usr.FatherName,
                    Hobbies = usr.Hobbies,
                    Login = usr.Login,
                    Id = usr.Id,
                    Interests = usr.Interests,
                    PhotoHash = usr.PhotoHash,
                    Skills = usr.Skills,
                    SurName = usr.SurName,
                    Contact = usr.Contact
                };
            })
            .ToList();
    }
}