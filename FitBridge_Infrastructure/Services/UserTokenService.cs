using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FitBridge_Application.Commons.Constants;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace FitBridge_Infrastructure.Services;

public class UserTokenService(
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager
    ) : IUserTokenService
{
    public string CreateAccessToken(ApplicationUser user, List<string> roles)
    {
        var jwtSettings = configuration.GetSection("JwtAccessTokenSettings");
        string secretKey = jwtSettings["Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptior = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, string.Join(",", roles)),
                ]),
            Expires = DateTime.Now.AddMinutes(configuration.GetValue<int>("JwtAccessTokenSettings:ExpirationInMinutes")),
            SigningCredentials = credentials,
            Audience = jwtSettings["Audience"],
            Issuer = jwtSettings["Issuer"]!,
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptior);

        return token;
    }

    public string CreateIdToken(ApplicationUser user, List<string> roles)
    {
        var jwtSettings = configuration.GetSection("JwtIDTokenSettings");
        string secretKey = jwtSettings["Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var audiences = jwtSettings.GetSection("Audience").Get<string[]>() ?? [];

        var claimsList = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Birthdate, user.Dob.ToString()!),
                new Claim(JwtRegisteredClaimNames.PhoneNumber, user.PhoneNumber!),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName!),
                new Claim("gymName", user.GymName ?? ""),
                new Claim(JwtRegisteredClaimNames.Gender, user.IsMale.ToString()!),
                new Claim("senderAvatar", user.AvatarUrl ?? ProjectConstant.defaultAvatar),
                new Claim("role", string.Join(",", roles)),
                new Claim("aud", audiences[0].ToString()),
                new Claim("aud", audiences[1].ToString()),
                new Claim("aud", audiences[2].ToString()),
            };

        var tokenDescriptior = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claimsList),
            Expires = DateTime.Now.AddMinutes(configuration.GetValue<int>("JwtIDTokenSettings:ExpirationInMinutes")),
            SigningCredentials = credentials,
            Issuer = jwtSettings["Issuer"]!,
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptior);

        return token;
    }

    public string CreateRefreshToken(ApplicationUser user)
    {
        var jwtSettings = configuration.GetSection("JwtRefreshTokenSettings");

        var number = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(number);
        var securityKey = new SymmetricSecurityKey(number);

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Claims = new Dictionary<string, object>
                    {
                        { JwtRegisteredClaimNames.Sub, user.Id.ToString() }
                    },
            Expires = DateTime.Now.AddMinutes(configuration.GetValue<int>("JwtRefreshTokenSettings:ExpirationInMinutes")),
            SigningCredentials = credentials,
            Issuer = jwtSettings["Issuer"]!,
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }

    public async Task RevokeUserToken(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found");
        }

        user.RefreshToken = null;
        await userManager.UpdateAsync(user);
    }
}
