namespace PizzaShop.Service.Implementations;

using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Implementations;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;
using PizzaShop.Service.Utils;

public class AuthService : IAuthService
{
    // private readonly IAuthRepository _authRepository;

    private readonly IUserRepository _userRepository;
    private readonly IMailService _mailService;

    public AuthService(IAuthRepository authRepository, IUserRepository userRepository, IMailService mailService)
    {
        // _authRepository = authRepository;
        _userRepository = userRepository;
        _mailService = mailService;
    }

    public async Task<User?> AuthenticateUser(string email, string password)  //Account instead of User
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user == null || !Utils.PasswordUtills.VerifyPassword(password, user.Password))
            return null;

        return user;
    }

    public async Task<User?> GetUser(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        return user;
    }

    public async Task SendForgotPasswordEmailAsync(string email, string resetLink, string filePath)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        string body = await System.IO.File.ReadAllTextAsync(filePath);
        body = body.Replace("{{reset_link}}", resetLink);
        if (user != null)
        {
            _mailService.SendMail(email, body, "To Reset Your Password");
        }
    }

    public async Task<string> ResetPassword(ResetPasswordModel model, string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        var isUserPasswor = PasswordUtills.VerifyPassword(model.NewPassword, user!.Password);
        if (user != null)
        {
            if (!isUserPasswor)
            {
                if (model.NewPassword == model.ConfirmNewPassword)
                {
                    user.Password = PasswordUtills.HashPassword(model.NewPassword);
                    await _userRepository.UpdateUserAsync(user);
                    return "success";
                }
                else
                {
                    return "New password must be match with confirm password.";
                }
            }
            else
            {
                return "New password must be different from the current password.";
            }
        }
        else
        {
            return "user not found";
        }
    }
}