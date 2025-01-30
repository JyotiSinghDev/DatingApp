using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController(DataContext _context , ITokenService tokenService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDto registerDto){
            if( await IfUserExist(registerDto.username.ToLower())) 
            return BadRequest("Username is taken.");
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDto.username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
                PasswordSalt =hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDTO{
                Username =user.UserName,
                Token = tokenService.CreateToken(user)
            };
        }

      [HttpPost("login")]
       public async Task<ActionResult<UserDTO>>Login(LoginDto loginDto)
       {
        var user = await _context.Users.FirstOrDefaultAsync(x=>x.UserName==loginDto.username.ToLower());

        if(user == null) return Unauthorized("Invalid User.");

        var hmac = new HMACSHA512(user.PasswordSalt);
        var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.password));
        for(int i=0; i< ComputeHash.Length;i++){
         if(ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password.");
        }

        return new UserDTO{
            Username =user.UserName,
            Token = tokenService.CreateToken(user)
        };
       }
        public async Task<bool> IfUserExist (string username){
           return await _context.Users.AnyAsync(x=>x.UserName.ToLower()==username.ToLower());
        }
    }
}
