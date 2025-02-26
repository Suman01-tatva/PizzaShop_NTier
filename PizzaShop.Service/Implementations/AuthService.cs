namespace PizzaShop.Service.Implementations;

using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzaShop.Entity.Data;
using PizzaShop.Repository.Implementations;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;

    public AuthService(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }

    public async Task<User?> AuthenticateUser(string email, string password)  //Account instead of User
    {
        var user = await _authRepository.GetUserByEmail(email);

        if (user == null || !Utils.PasswordUtills.VerifyPassword(password, user.Password))
            return null;

        return user;
    }

    public async Task<User?> GetUser(string email)
    {
        var user = await _authRepository.GetUserByEmail(email);
        return user;
    }

    public async Task SendForgotPasswordEmailAsync(string email, string resetLink, string filePath)
    {
        var user = await _authRepository.GetUserByEmail(email);

        string body = await System.IO.File.ReadAllTextAsync(filePath);
        body = body.Replace("{{reset_link}}", resetLink);
        if (user != null)
        {
            SendMail(email, resetLink, body);
        }
    }

    private void SendMail(string toEmail, string resetLink, string body)
    {
        string senderMail = "test.dotnet@etatvasoft.com";
        string senderPassword = "P}N^{z-]7Ilp";
        string host = "mail.etatvasoft.com";
        int port = 587;
        var smtpClient = new SmtpClient(host)
        {
            Port = port,
            Credentials = new NetworkCredential(senderMail, senderPassword),
            // EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(senderMail),
            Subject = "To Reset Your Password",
            // Body = $"Click <a href = '{resetLink}' > here </a> to reset your password",
            Body = body,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(toEmail);

        smtpClient.Send(mailMessage);
    }

    public async Task<Role?> CheckRole(string role)
    {
        return await _authRepository.CheckRole(role);
    }
}