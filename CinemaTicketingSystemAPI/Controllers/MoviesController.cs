using CinemaTicketingSystemAPI.BaseDb;
using CinemaTicketingSystemAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory; 

namespace CinemaTicketingSystemAPI.Controllers;
[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    public readonly AppDbContext _context;
    public readonly IMemoryCache _cache;
    public MoviesController(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        string cacheKey = "all_movies_chace";

        if (!_cache.TryGetValue(cacheKey, out List<Movie>? movies))
        {
            movies = await _context.Movies.ToListAsync();
            
            var chacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            
            _cache.Set(cacheKey, movies, chacheOptions);
            Console.WriteLine("Take the data from the DATABASE");
        }
        else
        {
            Console.WriteLine("The data from the CASH");
        }
        return Ok(movies);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Movie movie)
    {
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        _cache.Remove("all_movies_chace");
        
        return CreatedAtAction(nameof(GetAll), new { id = movie.Id }, movie);
    }
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, Movie updatedMovie)
    {
        if (id != updatedMovie.Id) return BadRequest("Invalid id");
        
        _context.Entry(updatedMovie).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok(updatedMovie);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null) return NotFound("Movie not found");

        _context.Movies.Remove(movie); 
        await _context.SaveChangesAsync();
        return NoContent();

    }
}