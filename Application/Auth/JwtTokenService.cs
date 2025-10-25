using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Back.Application.Auth;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly byte[] _signingKey;

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
        if (string.IsNullOrWhiteSpace(_settings.Secret))
        {
            throw new InvalidOperationException("JWT secret is not configured.");
        }

        _signingKey = Encoding.UTF8.GetBytes(_settings.Secret);
    }

    public string GenerateToken(string login)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, login),
            new(JwtRegisteredClaimNames.UniqueName, login)
        };

        var credentials = new SigningCredentials(new SymmetricSecurityKey(_signingKey), SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(_settings.ExpirationDays <= 0 ? 5 : _settings.ExpirationDays),
            Issuer = string.IsNullOrWhiteSpace(_settings.Issuer) ? null : _settings.Issuer,
            Audience = string.IsNullOrWhiteSpace(_settings.Audience) ? null : _settings.Audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
