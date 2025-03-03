using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Service.Interfaces;
using PizzaShop.Web.Models;

namespace PizzaShop.Web.Controllers;
// [Route("api/[controller]")]
// [ApiController]
public class MenuController : Controller
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    public async Task<IActionResult> Menu()
    {
        var categories = await _menuService.GetAllMenuCategoriesAsync();
        if (categories == null || !categories.Any())
        {
            return NotFound("No menu categories found.");
        }
        return View(categories);
    }

    public PartialViewResult ItemsByCategory(int categoryId)
    {
        var filteredItems = _menuService.GetItemsByCategory(categoryId);
        return PartialView("_ItemsList", filteredItems);
    }
}
