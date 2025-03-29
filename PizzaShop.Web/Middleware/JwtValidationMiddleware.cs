namespace PizzaShop.Web.Middleware;
using Microsoft.AspNetCore.Http;
using PizzaShop.Service.Interfaces;
using System.Threading.Tasks;

public class JwtValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public JwtValidationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Cookies["Token"]; // Fetch JWT from Cookies

        // Add Headers to Prevent Caching
        context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
        context.Response.Headers["Pragma"] = "no-cache";
        context.Response.Headers["Expires"] = "-1";

        if (!string.IsNullOrEmpty(token))
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var tokenDataService = scope.ServiceProvider.GetRequiredService<ITokenDataService>();
                var (email, id, isFirstLogin) = await tokenDataService.GetEmailFromToken(token);

                // If Token is valid
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(id))
                {
                    // Store claims in HttpContext
                    context.Items["UserId"] = id;
                    context.Items["UserEmail"] = email;
                    context.Items["IsFirstLogin"] = isFirstLogin;

                    await _next(context);
                    return;
                }
            }
        }

        // Redirect to Login if token is missing or invalid
        if (context.Request.Path != "/Auth/Login" && context.Request.Path != "/Auth/ResetPassword")
        {
            context.Response.Redirect("/Auth/Login");
            return;
        }

        await _next(context);
    }
}
