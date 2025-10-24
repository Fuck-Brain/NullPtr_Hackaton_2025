namespace Back.Application;

public record UserUpdateDto(string? Login, string? Password, string? Name, string? Surname, string? FatherName, int? Age, string? Gender, string? City, string? Contact, string? Skills, string? Description);