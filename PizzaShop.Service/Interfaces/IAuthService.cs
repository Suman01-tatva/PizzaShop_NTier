namespace PizzaShop.Service.Interfaces;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;

public interface IAuthService
{
    Task<User?> AuthenticateUser(string email, string password);
    Task<User?> GetUser(string email);
    Task<Role?> CheckRole(string role);

    Task<bool> ResetPassword(ResetPasswordModel model, string email);
    Task SendForgotPasswordEmailAsync(string email, string resetLink, string filePath);
}
