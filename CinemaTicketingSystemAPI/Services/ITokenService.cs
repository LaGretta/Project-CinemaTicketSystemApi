using CinemaTicketingSystemAPI.Models;

namespace CinemaTicketingSystemAPI.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}