using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;
// [Route("api/[controller]")]
// [ApiController]
public class MenuController : Controller
{
    private readonly IMenuService _menuService;
    private readonly IMenuModifierService _menuModifierService;
    public MenuController(IMenuService menuService, IMenuModifierService menuModifierService)
    {
        _menuService = menuService;
        _menuModifierService = menuModifierService;
    }

    [HttpGet]
    public async Task<IActionResult> Menu(int? id, int pageSize = 5, int pageIndex = 1, string searchString = "")
    {
        var categories = await _menuService.GetAllMenuCategoriesAsync();
        if (!categories.Any())
        {
            return NotFound("No categories found.");
        }

        var validCategoryId = id ?? categories.First().Id;
        var itemTabDetails = await _menuService.GetItemTabDetails(validCategoryId, pageSize, pageIndex, searchString);

        //modifierTab data
        var modifierGroups = await _menuModifierService.GetAllMenuModifierGroupAsync();

        var validModifierGroupId = id ?? modifierGroups.First().Id;
        var modifierTabDetails = await _menuModifierService.GetModifierTabDetails(validModifierGroupId);

        var model = new MenuViewModel
        {
            ItemTab = itemTabDetails,
            ModifierTab = modifierTabDetails
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(MenuCategoryViewModel model)
    {
        if (ModelState.IsValid)
        {
            var category = await _menuService.AddNewCategory(model.Name, model);

            if (category)
            {
                TempData["SuccessMessage"] = "Category created successfully.";
                return RedirectToAction(nameof(Menu));
            }
            else
            {
                TempData["ErrorMessage"] = "A category with this name already exists.";
            }
        }

        var categories = await _menuService.GetAllMenuCategoriesAsync();
        // var updatedodel = new MenuViewModel
        // {
        //     ItemTab = 
        // };
        return RedirectToAction("Menu", "Menu");
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
    public async Task<IActionResult> GetItemsByCategory(int categoryId, int pageSize, int pageIndex, string searchString = "")
    {
        List<MenuItemViewModel> items = await _menuService.GetItemsByCategory(categoryId, pageSize == 0 ? 5 : pageSize, pageIndex == 0 ? 1 : pageIndex, searchString);
        var totalPage = _menuService.GetItemsCountByCId(categoryId);
        var model = new ItemTabViewModel
        {
            itemList = items,
            PageSize = pageSize,
            PageIndex = pageIndex,
            SearchString = searchString,
            TotalPage = (int)Math.Ceiling(totalPage / (double)pageSize)
        };
        return PartialView("_ItemListPartial", model);
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

    [HttpGet]
    public JsonResult GetAllUnits()
    {
        var units = Json(_menuService.GetAllUnits());
        return units;
    }

    [HttpGet]
    public async Task<JsonResult> GetAllCategory()
    {
        var categories = Json(await _menuService.GetAllMenuCategoriesAsync());
        return categories;
    }

    [HttpGet]
    public IActionResult AddItem()
    {
        return PartialView("_AddItem", new MenuItemViewModel());
    }
}