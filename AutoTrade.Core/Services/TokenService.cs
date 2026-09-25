using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoTrade.Core.Configuration;
using AutoTrade.Core.DTOs;
using AutoTrade.Core.Entities;
using AutoTrade.Core.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AutoTrade.Service.Services;

public class TokenService : ITokenService
{
    private readonly CustomTokenOptions _tokenOptions;

    public TokenService(IOptions<CustomTokenOptions> tokenOptions)
    {
        _tokenOptions = tokenOptions.Value;
    }

    public TokenDto CreateToken(User user)
    {
        var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_tokenOptions.AccessTokenExpiration);
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenExpiration);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.SecurityKey));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var audience = _tokenOptions.Audience != null && _tokenOptions.Audience.Any()
            ? _tokenOptions.Audience.First()
            : string.Empty;

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: accessTokenExpiration,
            signingCredentials: signingCredentials
        );

        var handler = new JwtSecurityTokenHandler();
        var accessToken = handler.WriteToken(jwtSecurityToken);

        return new TokenDto
        {
            AccessToken = accessToken,
            AccessTokenExpiration = accessTokenExpiration,
            RefreshToken = CreateRefreshToken(),
            RefreshTokenExpiration = refreshTokenExpiration
        };
    }

    private static string CreateRefreshToken()
    {
        var numberBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(numberBytes);
        return Convert.ToBase64String(numberBytes);
    }
}