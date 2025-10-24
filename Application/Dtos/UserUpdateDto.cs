namespace Back.Application;

public class UserUpdateDto
{
    public string? Login { get; set; } = null;
    public string? Password { get; set; } = null;
    public string? Name { get; set; } = null;
    public string? Surname { get; set; } = null;
    public string? FatherName { get; set; } = null;
    public int? Age { get; set; } = null;
    public string? Gender { get; set; } = null;
    public string? City { get; set; } = null;
    public string? Contact { get; set; } = null;
    public List<string>? Skills { get; set; } = null;
    public List<string>? Interests { get; set; } = null;
    public List<string>? Hobbies { get; set; } = null;
    public string? Description { get; set; } = null;
}