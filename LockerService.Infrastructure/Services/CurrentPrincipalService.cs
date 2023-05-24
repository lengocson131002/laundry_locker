using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LockerService.Application.Common.Enums;
using LockerService.Application.Common.Exceptions;
using LockerService.Application.Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LockerService.Infrastructure.Services;

public class CurrentPrincipalService : ICurrentPrincipalService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly IConfiguration _configuration;

    public CurrentPrincipalService(IHttpContextAccessor accessor, IConfiguration configuration)
    {
        _accessor = accessor;
        _configuration = configuration;
    }

    // Get current login acc Id
    public string? CurrentPrincipal
    {
        get
        {
            var identity = _accessor?.HttpContext?.User.Identity as ClaimsIdentity;
                
            if (identity == null || !identity.IsAuthenticated) return null;

            var claims = identity.Claims;

            var id = claims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value ?? null;

            return id;
        }
    }

    public int? CurrentSubjectId => CurrentPrincipal != null ? int.Parse(CurrentPrincipal) : null;
    
    public ClaimsPrincipal GetCurrentPrincipalFromToken(string token)
    {
        var tokenValidationParams = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ??
                                                                               throw new ArgumentException(
                                                                                   "Jwt:Key is required")))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParams, out var securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new ApiException(ResponseCode.AuthErrorInvalidRefreshToken);
        }

        return principal;
    }
}