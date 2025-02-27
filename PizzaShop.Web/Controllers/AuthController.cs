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
                ModelState.AddModelError("InvalidCredentials", "Please enter valid credentials");
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

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        string userEmail = DecryptToken(token);
        TempData["Email"] = userEmail;
        if (userEmail == null)
        {
            TempData["ToastMessage"] = "Reset Password Link is expired!!!";
            TempData["ToastType"] = "error";
            TempData["ToastTitle"] = "!";
            return RedirectToAction("ForgotPassword", "Auth");
        }
        return View();
    }

    private string DecryptToken(string token)
    {
        try
        {
            byte[] inputBytes = Convert.FromBase64String(token);

            string SecretKey = "hello7hisisP1zzaShop";
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(SecretKey.PadRight(32));
                aes.IV = new byte[16];

                var decryptor = aes.CreateDecryptor();
                byte[] decryptedBytes = decryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                string data = Encoding.UTF8.GetString(decryptedBytes);

                string[] parts = data.Split('|');
                if (parts.Length != 2 || DateTime.UtcNow > DateTime.Parse(parts[1]))
                {
                    return null;
                }
                return parts[0];
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return null;
        }
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            string? email = TempData["Email"]?.ToString();
            if (email == null)
            {
                TempData["ErrorMessage"] = "Reset Password Link is expired or invalid.";
                return RedirectToAction("ForgotPassword", "Auth");
            }

            var isResetPassword = await _authService.ResetPassword(model, email);
            if (isResetPassword)
            {
                TempData["SeccessMessage"] = "Password reset successfully. Please login with your new password.";
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                TempData["FailMessage"] = "Failed to reset password. Please try again.";
                return View(model);
            }
        }
        return View(model);
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("Token");
        Response.Cookies.Delete("UserData");
        Response.Cookies.Delete("email");
        return RedirectToAction("Login", "Auth");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
