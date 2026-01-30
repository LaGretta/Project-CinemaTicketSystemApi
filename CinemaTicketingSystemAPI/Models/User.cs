namespace CinemaTicketingSystemAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Client";

    public List<Ticket> Tickets { get; set; } = new();
}
public class Ticket
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
   
    public int MovieId { get; set; }
    public Movie? Movie { get; set; }
    
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Active";
    
}
public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int DurationInMinutes { get; set; }
    public string Description { get; set; } = string.Empty;
}