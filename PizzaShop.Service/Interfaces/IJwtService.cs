using System.Security.Claims;

namespace PizzaShop.Service.Interfaces;

public interface IJwtService
{
    string GenerateJwtToken(string id, string email, string role);
    object GenerateJwtToken(string email);

    ClaimsPrincipal? ValidateToken(string token);
}
