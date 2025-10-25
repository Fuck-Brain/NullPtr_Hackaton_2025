namespace Back.Application.Auth;

public interface IJwtTokenService
{
    string GenerateToken(string login);
}
