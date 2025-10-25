namespace Back.API.DTO;

public class AuthResponseDto
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
}