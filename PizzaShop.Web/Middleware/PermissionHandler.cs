using Microsoft.AspNetCore.Authorization;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Middleware;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IRolePermissionService _roleService;
    private readonly ITokenDataService _tokenDataService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionHandler(IRolePermissionService roleService, ITokenDataService tokenDataService, IHttpContextAccessor httpContextAccessor)
    {
        this._roleService = roleService;
        this._tokenDataService = tokenDataService;
        this._httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var token = httpContext!.Request.Cookies["Token"];
        var (email, roleId, isFirstLogin) = await _tokenDataService.GetEmailFromToken(token!);
        var permissionsData = _roleService.GetRolePermissionByRoleId(int.Parse(roleId));

        switch (requirement.Permission)
        {
            case "User.View":
                if (permissionsData[0].CanView == true)
                    context.Succeed(requirement);
                break;
            case "User.AddEdit":
                if (permissionsData[0].CanEdit == true)
                    context.Succeed(requirement);
                break;
            case "User.Delete":
                if (permissionsData[0].CanDelete == true)
                    context.Succeed(requirement);
                break;
            case "Role.View":
                if (permissionsData[1].CanView == true)
                    context.Succeed(requirement);
                break;
            case "Role.AddEdit":
                if (permissionsData[1].CanEdit == true)
                    context.Succeed(requirement);
                break;
            case "Role.Delete":
                if (permissionsData[1].CanDelete == true)
                    context.Succeed(requirement);
                break;
            case "Menu.View":
                if (permissionsData[2].CanView == true)
                    context.Succeed(requirement);
                break;
            case "Menu.AddEdit":
                if (permissionsData[2].CanEdit == true)
                    context.Succeed(requirement);
                break;
            case "Menu.Delete":
                if (permissionsData[2].CanDelete == true)
                    context.Succeed(requirement);
                break;
            case "TableSection.View":
                if (permissionsData[3].CanView == true)
                    context.Succeed(requirement);
                break;
            case "TableSection.AddEdit":
                if (permissionsData[3].CanEdit == true)
                    context.Succeed(requirement);
                break;
            case "TableSection.Delete":
                if (permissionsData[3].CanDelete == true)
                    context.Succeed(requirement);
                break;
            case "TaxFees.View":
                if (permissionsData[4].CanView == true)
                    context.Succeed(requirement);
                break;
            case "TaxFees.AddEdit":
                if (permissionsData[4].CanEdit == true)
                    context.Succeed(requirement);
                break;
            case "TaxFees.Delete":
                if (permissionsData[4].CanDelete == true)
                    context.Succeed(requirement);
                break;
            case "Orders.View":
                if (permissionsData[5].CanView == true)
                    context.Succeed(requirement);
                break;
            case "Orders.AddEdit":
                if (permissionsData[5].CanEdit == true)
                    context.Succeed(requirement);
                break;
            case "Orders.Delete":
                if (permissionsData[5].CanDelete == true)
                    context.Succeed(requirement);
                break;
            case "Customers.View":
                if (permissionsData[6].CanView == true)
                    context.Succeed(requirement);
                break;
            case "Customers.AddEdit":
                if (permissionsData[6].CanEdit == true)
                    context.Succeed(requirement);
                break;
            case "Customers.Delete":
                if (permissionsData[6].CanDelete == true)
                    context.Succeed(requirement);
                break;
            default:
                break;
        }
        return Task.CompletedTask;
    }
}
