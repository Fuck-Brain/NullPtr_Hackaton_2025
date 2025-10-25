namespace Back.API.DTO;

public class UserRegisterDTO
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string PhotoHash { get; set; }
    public string Name { get; set; }
    public string SurName { get; set; }
    public string FatherName { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public string City { get; set; }
    public string Contact { get; set; }
}