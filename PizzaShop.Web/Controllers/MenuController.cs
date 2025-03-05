using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

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

    [HttpGet]
    public async Task<IActionResult> Menu(int? categoryId)
    {
        var categories = await _menuService.GetAllMenuCategoriesAsync();
        if (!categories.Any())
        {
            return NotFound("No categories found.");
        }

        var validCategoryId = categoryId ?? categories.First().Id;
        var itemTabDetails = await _menuService.GetItemTabDetails(validCategoryId);

        var model = new MenuViewModel
        {
            ItemTab = itemTabDetails
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult AddNewCategory(string Name, string Description)
    {
        if (ModelState.IsValid)
        {
            bool result = _menuService.AddNewCategory(Name, Description);
            if (result)
            {
                return Json(new { success = true, message = "New Category Added" });
                // return View("Menu", "Menu");
            }
            else
            {
                return Json(new { success = false, message = "An error occurred while adding the category." });
            }
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EditCategory(int id)
    {
        var category = await _menuService.GetCategoryDetailById(id);
        if (category == null)
        {
            return NotFound();
        }
        return PartialView("_EditCategory", category);
    }

    [HttpPost]
    public async Task<IActionResult> EditCategory(MenuCategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _menuService.EditCategory(model, model.Id);

            if (result)
            {
                TempData["SuccessMessage"] = "Category updated successfully.";
                // return RedirectToAction(nameof(Menu));
                return Json(new { success = true, message = "Category updated successfully." });
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update category.";
                return Json(new { success = false, message = "Failed to update category." });
            }
        }

        return Json(new { success = false, message = "Invalid data." });
    }

    [HttpGet]
    public async Task<IActionResult> GetItemsByCategory(int categoryId)
    {
        List<MenuItemViewModel> filteredItems = await _menuService.GetItemsByCategory(categoryId);
        return PartialView("_ItemListPartial", filteredItems);
    }

    [HttpPost]
    public IActionResult DeleteCategory(int id)
    {
        var result = _menuService.SoftDeleteCategory(id);
        if (result)
        {
            return Json(new { success = true, message = "Category deleted successfully." });
        }
        else
        {
            return Json(new { success = false, message = "Failed to delete category." });
        }
    }
}