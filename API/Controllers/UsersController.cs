using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class UsersController(DataContext context) : BaseApiController
{
    [HttpGet]

    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await context.User.ToListAsync();

        return users;
    }


    [HttpGet("{id:int}")] // /api/users/2

    public async Task<ActionResult<AppUser>> GetUsers(int id)
    {
        var user = await context.User.FindAsync(id);

        if(user == null) return NotFound();

        return user;
    }
}
