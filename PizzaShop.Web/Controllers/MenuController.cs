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
    public IActionResult AddNewCategory(MenuCategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            bool result = _menuService.AddNewCategory(model);
            if (result)
            {
                return Json(new { success = true, message = "New Category Added" });
            }
            else
            {
                return Json(new { success = false, message = "An error occurred while adding the category." });
            }
        }
        // else
        // {
        //     return Json(new { success = false, message = "Enter Proper Details of Category" });
        // }
        return View();
    }

    // [HttpPost]
    // public async Task<IActionResult> AddNewCategory(MenuCategoryViewModel model)
    // {
    //     var result = _menuService.AddNewCategory(model);

    //     if (result)
    //     {
    //         return Json(new { success = true, message = "Category added successfully." });
    //     }
    //     // else
    //     // {
    //     //     return Json(new { success = false, message = "Failed to add category." });
    //     // }
    //     return View();
    // }

    [HttpGet]
    public async Task<IActionResult> EditCategory(int id)
    {
        var category = await _menuService.GetCategoryDetailById(id);
        if (category == null)
        {
            return NotFound();
        }

        return PartialView("_EditCategoryPartial", category);
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
                return RedirectToAction(nameof(Menu));
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update category.";
            }
        }

        return View(model);
    }
    public IActionResult ItemsByCategory(int categoryId)
    {
        var filteredItems = _menuService.GetItemsByCategory(categoryId);
        return PartialView("_ItemsListPartial", filteredItems);
    }
}
