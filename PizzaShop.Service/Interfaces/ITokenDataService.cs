namespace PizzaShop.Service.Interfaces;

public interface ITokenDataService
{
    Task<(string email, string id)> GetEmailFromToken(string token);
}