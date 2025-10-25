using Back.Domain.Entity;

namespace Back.Application.Dtos;

public class UserMatchDto
{
    public Guid Id { get;  set; }
    public string Login { get;  set; }
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
    public ICollection<string> Skills { get;  set; } = new List<string>();
    public ICollection<string> Interests { get;  set; } = new List<string>();
    public ICollection<string> Hobbies { get;  set; } = new List<string>();
}