using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")] //account/register

    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {

        if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");
        return Ok();
        // using var hmac = new HMACSHA512();

        // var user = new AppUser
        // {
        //     UserName = registerDto.Username.ToLower(),
        //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
        //     PasswordSalt = hmac.Key
        // };

        // context.User.Add(user);
        // await context.SaveChangesAsync();
        
        // return new UserDto
        // {
        //     username  = user.UserName,
        //     Token = tokenService.CreateToken(user)
        // };
    }

    [HttpPost("Login")]

    public async Task<ActionResult<UserDto>> Login(LoginDto  loginDto){
        var user = await context.User.FirstOrDefaultAsync(x => 
        x.UserName == loginDto.Username.ToLower());

        if(user == null) return Unauthorized("Invalid username");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        
        for (int i = 0; i < computedHash.Length; i++)
        {
            if( computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }
        
        return new UserDto
        {
            username  = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }

    public async Task<bool> UserExists(string Username){
        return await context.User.AnyAsync(x => x.UserName.ToLower() == Username.ToLower()); //Bob != bob
    }
}
