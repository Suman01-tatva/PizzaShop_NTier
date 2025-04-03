using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Models;

namespace PizzaShop.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRolePermissionService _rolePermissionService;
    private readonly ITokenDataService _tokenDataService;
    public HomeController(ILogger<HomeController> logger, IRolePermissionService rolePermissionService, ITokenDataService tokenDataService)
    {
        _logger = logger;
        _rolePermissionService = rolePermissionService;
        _tokenDataService = tokenDataService;
    }

    public async Task<IActionResult> AdminDashboard()
    {
        var token = Request.Cookies["Token"];
        var roleId = await _tokenDataService.GetRoleFromToken(token!);
        Console.WriteLine("Role Id", roleId);
        var permission = _rolePermissionService.GetRolePermissionByRoleId(roleId);
        HttpContext.Session.SetString("permission", JsonConvert.SerializeObject(permission));
        return View();
    }

    public async Task<IActionResult> UserDashboard()
    {
        var token = Request.Cookies["Token"];
        var roleId = await _tokenDataService.GetRoleFromToken(token!);
        Console.WriteLine("Role Id", roleId);
        var permission = _rolePermissionService.GetRolePermissionByRoleId(roleId);
        HttpContext.Session.SetString("permission", JsonConvert.SerializeObject(permission));
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    public IActionResult NotFound()
    {
        return View("404");
    }
}
