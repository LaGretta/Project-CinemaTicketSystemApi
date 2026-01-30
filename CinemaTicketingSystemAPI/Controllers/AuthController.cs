using CinemaTicketingSystemAPI.BaseDb;
using CinemaTicketingSystemAPI.Models;
using CinemaTicketingSystemAPI.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketingSystemAPI.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;
    public AuthController(AppDbContext dbContext, ITokenService tokenService)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(User user)
    {
        if(await _dbContext.Users.AnyAsync(x => x.Email == user.Email))
            return BadRequest("Email already exists");
        
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return Ok("Success");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(n => n.Email == request.Email);
        
        if(user == null || user.PasswordHash != request.Password)
            return Unauthorized("Invalid username or password");
        
        var token = _tokenService.GenerateToken(user);
        return Ok(new {Token = token});
    }
}
