using System.Security.Claims;
using CinemaTicketingSystemAPI.BaseDb;
using CinemaTicketingSystemAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketingSystemAPI.Controllers;
[ApiController]
[Route("api/tickets")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly AppDbContext _context;
    public TicketsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> BuyTicket(int movieId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var ticket = new Ticket
        {
            UserId = userId,
            MovieId = movieId,
            PurchaseDate = DateTime.Now,
            Status = "Active"
        };
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return Ok("Ticket bought");
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllTickets()
    {
        var allTickets = await _context.Tickets
            .Include(t => t.Movie)
            .Include(t => t.User)
            .ToListAsync();
        return Ok(allTickets);
    }
    
    [HttpGet("user")]
    public async Task<IActionResult> GetMyTickets()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var myTickets = await _context.Tickets
            .Where(t => t.UserId == userId)
            .Include(t => t.Movie)
            .ToListAsync();
        
        return Ok(myTickets);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelTicket(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        
        if(ticket == null) return NotFound("Ticket not found");
        
        ticket.Status = "Cancelled";
        await _context.SaveChangesAsync();
        return Ok("Ticket cancelled");
    }
}