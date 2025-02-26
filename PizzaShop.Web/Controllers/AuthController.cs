using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;
using PizzaShop.Service.Utils;
using PizzaShop.Web.Models;
using System.Security.Cryptography;
using NuGet.Common;

namespace PizzaShop.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService? _authService;
    private readonly IJwtService? _jwtService;

    public AuthController(IAuthService authService, IJwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    public IActionResult Login()
    {
        var token = Request.Cookies["Token"];
        var ValidateToken = _jwtService?.ValidateToken(token!);
        if (ValidateToken != null)
        {
            return RedirectToAction("AdminDashBoard", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _authService.AuthenticateUser(model.Email, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Please enter valid credentials");
                return View("Login");
            }

            // Generate JWT Token
            var token = _jwtService.GenerateJwtToken(user.Id.ToString(), user.Email, user.RoleId.ToString());

            // Store token in cookie
            CookieUtils.SaveJWTToken(Response, token);
            Response.Cookies.Append("email", user.Email);

            // Save User Data to Cookie for Remember Me functionality.
            if (model.RememberMe)
            {
                CookieUtils.SaveUserData(Response, user);
            }
            TempData["Email"] = user.Email;

            if (user.RoleId == 1)
            {
                return RedirectToAction("AdminDashboard", "Home");
            }
            else
            {
                return RedirectToAction("UserDashboard", "Home");
            }
        }
        return View("Login");
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            string token = GenerateToken(model.Email);
            string resetLink = Url.Action("ResetPassword", "Auth", new { token }, Request.Scheme);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/ResetPasswordEmail.html");

            if (!System.IO.File.Exists(filePath))
            {
                ModelState.AddModelError(string.Empty, "Email body template not found");
            }
            await _authService.SendForgotPasswordEmailAsync(model.Email, resetLink, filePath);

            return View("ForgotPasswordConfirmation");
        }
        return View();
    }

    private string GenerateToken(string email)
    {
        string data = $"{email}|{DateTime.UtcNow.AddHours(1)}";
        string SecretKey = "hello7hisisP1zzaShop";
        using (var aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(SecretKey.PadRight(32));
            aes.IV = new byte[16];

            var encryptor = aes.CreateEncryptor();
            byte[] inputBytes = Encoding.UTF8.GetBytes(data);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            return Convert.ToBase64String(encryptedBytes);
        }
    }

    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    public IActionResult RestPassword(Token token)
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
