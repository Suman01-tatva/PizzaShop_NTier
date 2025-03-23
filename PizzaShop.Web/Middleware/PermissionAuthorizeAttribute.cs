using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PizzaShop.Web.Middleware;

public class PermissionAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _permission;

    public PermissionAuthorizeAttribute(string permission) : base()
    {
        Policy = $"{permission}";
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        if (!user.HasClaim("Permission", _permission))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
        }
    }
}