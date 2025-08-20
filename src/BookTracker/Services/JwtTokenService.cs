using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookTracker.Common;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BookTracker.Services;

public class JwtTokenService(IOptions<ApplicationSetting> options)
{
    private readonly JwtConfiguration _jwtConfiguration = options.Value.JwtConfiguration;
    
    public AccessTokenModel GenerateToken(string userId, string phoneNumber)
    {
        var claims = new[]
        {
            new Claim(JwtClaimTypes.UserId, userId),
            new Claim(JwtClaimTypes.PhoneNumber, phoneNumber),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtConfiguration.Issuer,
            audience: _jwtConfiguration.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtConfiguration.ExpireMinute),
            signingCredentials: signingCredentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        var expireTimestamp = (int)(jwtSecurityToken.ValidTo - DateTime.Now).TotalSeconds;

        return new AccessTokenModel(token, expireTimestamp);

    }
}

public record AccessTokenModel(string Token, int Expire);