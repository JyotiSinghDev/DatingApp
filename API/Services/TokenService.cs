using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"]??throw new Exception("Cannot access token  key from appsettings.");
        if(tokenKey.Length<64) throw new Exception("Your token should be longer.");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserName)
                };
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokeDescriptor = new SecurityTokenDescriptor{
                
                Subject= new ClaimsIdentity(claims),
                Expires= DateTime.UtcNow.AddDays(7),
                SigningCredentials = signinCredentials
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokeDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
