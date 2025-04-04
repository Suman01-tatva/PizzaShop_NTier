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

    [AllowAnonymous]
    // [Route("/[controller]/login")]
    public IActionResult Login()
    {
        var token = Request.Cookies["Token"];
        var user = Request.Cookies["UserData"];
        var ValidateToken = _jwtService?.ValidateToken(token!);
        if (ValidateToken != null && user != null)
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
            var user = await _authService!.AuthenticateUser(model.Email.Trim(), model.Password);

            if (user == null)
            {
                ModelState.AddModelError("InvalidCredentials", "Please enter valid credentials");
                return View("Login");
            }
            var token = _jwtService!.GenerateJwtToken(user!.Id.ToString(), user.Email, user.RoleId.ToString(), user.IsFirstLogin);
            CookieUtils.SaveJWTToken(Response, token);
            if (user.IsDeleted == true)
            {
                TempData["ToastrMessage"] = "Your account is Deleted. Please contact support team.";
                TempData["ToastrType"] = "error";
                return View();
            }
            if (user.IsActive == false)
            {
                TempData["ToastrMessage"] = "Your account is inactive. Please contact support team to reactivate your account.";
                TempData["ToastrType"] = "error";
                return View();
            }
            if (user.IsFirstLogin == true)
            {
                return RedirectToAction("ChangePassword", "User");
            }

            if (model.RememberMe)
            {
                CookieUtils.SaveUserData(Response, user);
            }
            TempData["Email"] = user.Email;
            if (user.RoleId == 1 || user.RoleId == 2)
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
        var token = Request.Cookies["Token"];
        var user = Request.Cookies["UserData"];
        var ValidateToken = _jwtService?.ValidateToken(token!);
        if (ValidateToken != null && user != null)
        {
            return RedirectToAction("AdminDashBoard", "Home");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _authService!.GetUser(model.Email);
            if (user == null)
            {
                TempData["ToastrMessage"] = "User not Exist!!";
                TempData["ToastrType"] = "error";
                return View();
            }
            string token = CookieUtils.GenerateTokenForResetPassword(model.Email);
            string resetLink = Url.Action("ResetPassword", "Auth", new { token }, Request.Scheme)!;
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/ResetPasswordEmail.html");
            Response.Cookies.Append("ResetPasswordToken", token);
            if (!System.IO.File.Exists(filePath))
            {
                ModelState.AddModelError(string.Empty, "Email body template not found");
            }
            await _authService!.SendForgotPasswordEmailAsync(model.Email, resetLink, filePath);

            return View("ForgotPasswordConfirmation");
        }
        return View();
    }

    public IActionResult ForgotPasswordConfirmation()
    {
        var token = Request.Cookies["Token"];
        var user = Request.Cookies["UserData"];
        var ValidateToken = _jwtService?.ValidateToken(token!);
        if (ValidateToken != null && user != null)
        {
            return RedirectToAction("AdminDashBoard", "Home");
        }
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        if (token == null)
        {
            TempData["ToastrMessage"] = "Invalid link of ResetPassword!";
            TempData["ToastrType"] = "error";
            return RedirectToAction("ForgotPassword", "Auth");
        }
        var ResetPswToken = Request.Cookies["ResetPasswordToken"];
        if (ResetPswToken == null)
        {
            TempData["ToastrMessage"] = "You can change the passwrd only one time by this link.";
            TempData["ToastrType"] = "error";
            return RedirectToAction("ForgotPassword", "Auth");
        }
        string userEmail = DecryptToken(token);
        TempData["Email"] = userEmail;
        if (userEmail == "")
        {
            TempData["ToastrMessage"] = "Reset Password Link is expired Try Again!!";
            TempData["ToastrType"] = "error";
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
                    TempData["ErrorMessage"] = "Reset Password Link is expired or invalid.";
                    return "";
                }
                return parts[0];
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return "null";
        }
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            // string? email = TempData["Email"]?.ToString();
            // if (email == null)
            // {
            //     TempData["ToastrMessage"] = "Invalid link of ResetPassword!";
            //     TempData["ToastrType"] = "error";
            //     return RedirectToAction("ForgotPassword", "Auth");
            // }
            var token = Request.Cookies["ResetPasswordToken"];
            string email = DecryptToken(token);
            var resetPassword = await _authService!.ResetPassword(model, email);
            if (resetPassword == "success")
            {
                TempData["ToastrMessage"] = "Password reset successfully. Please login with your new password.";
                TempData["ToastrType"] = "success";
                Response.Cookies.Delete("ResetPasswordToken");
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                TempData["ToastrMessage"] = resetPassword;
                TempData["ToastrType"] = "error";
                return View(model);
            }
        }
        return View(model);
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("Token");
        Response.Cookies.Delete("UserData");
        HttpContext.Session.Clear();

        Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        Response.Headers["Pragma"] = "no-cache";
        Response.Headers["Expires"] = "-1";
        return RedirectToAction("Login", "Auth");
    }

    [HttpGet]
    public IActionResult Error(int statusCode)
    {
        var refererUrl = Request.Headers["Referer"].ToString(); // Referer header is used to get the URL of the previous page from which request(Error) comes.

        // if (statusCode == 403 && !string.IsNullOrEmpty(refererUrl))
        // {
        //     TempData["ToastrMessage"] = "You are not authorized to access this page.";
        //     TempData["ToastrType"] = "error";
        //     return Redirect(refererUrl);
        // }
        if (statusCode == 403)
        {
            return RedirectToAction("NotFound", "Home"); // ✅ Redirect to the 404 page instead of referer
        }

        if (statusCode == 401)
        {
            return RedirectToAction("NotFound", "Home"); // ✅ Show 404 for unauthorized access
        }
        // if (statusCode == 500 && !string.IsNullOrEmpty(refererUrl))
        // {
        //     TempData["error"] = "An error occurred while processing your request.";
        //     return Redirect(refererUrl);
        // }

        if (statusCode == 401 && !string.IsNullOrEmpty(refererUrl))
        {
            // TempData["ToastrMessage"] = "You are not authenticated Please Login.";
            // TempData["ToastrType"] = "error";
            return RedirectToAction("NotFound", "Home");
        }

        string errorText = statusCode switch
        {
            // 403 => "Forbidden",
            404 => "Page Not Found",
            500 => "Internal Server Error",
            _ => "Unexpected Error"
        };

        string subText = statusCode switch
        {
            // 403 => "You are not authorized to access this page.",
            404 => "The page you are looking for does not exist.",
            500 => "An error occurred while processing your request.",
            _ => "Unexpected error occurred."
        };

        return View(new ErrorViewModel { statusCode = statusCode, errorText = errorText, subText = subText });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
