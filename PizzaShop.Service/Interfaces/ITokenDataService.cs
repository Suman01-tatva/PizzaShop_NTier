namespace PizzaShop.Service.Interfaces;

public interface ITokenDataService
{
    Task<string> GetEmailFromToken(string token);
}