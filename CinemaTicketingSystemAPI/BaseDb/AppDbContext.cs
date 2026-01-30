using CinemaTicketingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketingSystemAPI.BaseDb;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    
    public DbSet<User>  Users { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tickets)
            .HasForeignKey(t => t.UserId);
    }
}