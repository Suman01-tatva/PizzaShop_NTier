using Microsoft.AspNetCore.Http;
using PizzaShop.Entity.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PizzaShop.Service.Utils
{
    public static class CookieUtils
    {
        public static void SaveJWTToken(HttpResponse response, string token)
        {
            response.Cookies.Append("Token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });
        }

        public static string? GetJWTToken(HttpRequest request)
        {
            _ = request.Cookies.TryGetValue("SuperSecretAuthToken", out string? token);
            return token;
        }

        public static void SaveUserData(HttpResponse response, User user)
        {
            var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
            string userData = JsonSerializer.Serialize(user, options);

            // Store user details in a cookie for 3 days
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(3),
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            };
            response.Cookies.Append("UserData", userData, cookieOptions);
        }

        public static Account? GetUserData(HttpRequest request)
        {
            var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };

            // Store user details in a cookie for 3 days
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(3),
                HttpOnly = true,
                Secure = true,
                IsEssential = true
            };
            var data = request.Cookies["userData"];
            return string.IsNullOrEmpty(data) ? null : JsonSerializer.Deserialize<Account>(data, options);
        }

        public static void ClearCookies(HttpContext httpContext)
        {
            httpContext.Response.Cookies.Delete("SuperSecretAuthToken");
            httpContext.Response.Cookies.Delete("UserData");
        }
    }
}