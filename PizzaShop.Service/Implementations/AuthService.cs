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

    public AuthService(IAuthRepository authRepository, IUserRepository userRepository)
    {
        // _authRepository = authRepository;
        _userRepository = userRepository;
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
            SendMail(email, body);
        }
    }

    private void SendMail(string toEmail, string body)
    {
        string senderMail = "test.dotnet@etatvasoft.com";
        string senderPassword = "P}N^{z-]7Ilp";
        string host = "mail.etatvasoft.com";
        int port = 587;
        var smtpClient = new SmtpClient(host)
        {
            Port = port,
            Credentials = new NetworkCredential(senderMail, senderPassword),
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderMail),
            Subject = "To Reset Your Password",
            Body = body,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(toEmail);

        smtpClient.Send(mailMessage);
    }

    public async Task<bool> ResetPassword(ResetPasswordModel model, string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user != null)
        {
            if (model.NewPassword != null && model.ConfirmNewPassword != null)
            {
                if (model.NewPassword == model.ConfirmNewPassword)
                {
                    user.Password = PasswordUtills.HashPassword(model.NewPassword);
                    await _userRepository.UpdateUserAsync(user);
                    return true;
                }
            }
        }
        return false;
    }
}