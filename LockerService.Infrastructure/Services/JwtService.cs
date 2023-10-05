using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LockerService.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using JwtClaims = LockerService.Application.Common.Constants.JwtClaims;

namespace LockerService.Infrastructure.Services;

public class JwtService : IJwtService
{

    private readonly JwtSettings _jwtSettings;

    public JwtService(IConfiguration configuration, JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    private string GenerateJwtToken(Account account, int expireInMinutes)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new Claim(ClaimTypes.Name, account.FullName ?? string.Empty),
            new Claim(ClaimTypes.Role, account.Role.ToString())
        };

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expireInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GenerateJwtToken(Account account)
    {
        return GenerateJwtToken(account, _jwtSettings.TokenExpire);
    }

    public string GenerateJwtRefreshToken(Account account)
    {
        return GenerateJwtToken(account, _jwtSettings.RefreshTokenExpire);
    }

    public string RevokeJwtRefreshToken(Account account)
    {
        throw new NotImplementedException();
    }
}