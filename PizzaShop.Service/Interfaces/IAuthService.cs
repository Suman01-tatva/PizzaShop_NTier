namespace PizzaShop.Service.Interfaces;
using PizzaShop.Entity.Data;


public interface IAuthService
{
    Task<User?> AuthenticateUser(string email, string password);
    Task<User?> GetUser(string email);
    Task<Role?> CheckRole(string role);
    Task SendForgotPasswordEmailAsync(string email, string resetLink, string filePath);
}
