namespace PizzaShop.Service.Interfaces;

public interface ITokenDataService
{
    Task<(string email, string id, string isFirstLogin)> GetEmailFromToken(string token);
}