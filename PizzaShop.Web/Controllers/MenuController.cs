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
    private readonly ITokenDataService _tokenDataService;
    public MenuController(IMenuService menuService, IMenuModifierService menuModifierService, ITokenDataService tokenDataService)
    {
        _menuService = menuService;
        _menuModifierService = menuModifierService;
        _tokenDataService = tokenDataService;
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
                TempData["ToastrMessage"] = "Category created successfully.";
                TempData["ToastrType"] = "success";
                return RedirectToAction("Menu", "Menu");
            }
            else
            {
                TempData["ToastrMessage"] = "A category with this name already exists.";
                TempData["ToastrType"] = "error";
                return RedirectToAction("Menu", "Menu");
            }
        }

        var categories = await _menuService.GetAllMenuCategoriesAsync();

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
                TempData["ToastrMessage"] = "Category updated successfully.";
                TempData["ToastrType"] = "success";
                return Json(new { success = true, message = "Category updated successfully.", redirectUrl = Url.Action("Menu") });
            }
            else
            {
                TempData["ToastrMessage"] = "Failed to update the Category. Please try again.";
                TempData["ToastrType"] = "error";
                return Json(new { success = false, message = "Failed to update category.", redirectUrl = Url.Action("Menu") });
            }
        }

        return Json(new { success = false, message = "Invalid data." });
    }

    [HttpGet]
    public async Task<IActionResult> GetItemsByCategory(int categoryId, int pageSize, int pageIndex, string searchString = "")
    {
        List<MenuItemViewModel> items = await _menuService.GetItemsByCategory(categoryId, pageSize == 0 ? 5 : pageSize, pageIndex == 0 ? 1 : pageIndex, searchString);
        var totalItemCount = _menuService.GetItemsCountByCId(categoryId);
        Console.WriteLine("Totalpage:", totalItemCount);
        var model = new ItemTabViewModel
        {
            itemList = items,
            PageSize = pageSize,
            PageIndex = pageIndex,
            SearchString = searchString,
            TotalPage = (int)Math.Ceiling(totalItemCount / (double)pageSize)
        };
        return PartialView("_ItemListPartial", model);
    }

    [HttpPost]
    public IActionResult DeleteCategory(int id)
    {
        var result = _menuService.SoftDeleteCategory(id);
        if (result)
        {
            TempData["ToastrMessage"] = "Category deleted successfully.";
            TempData["ToastrType"] = "success";
            return Json(new { success = true, message = "Category deleted successfully." });
        }
        else
        {
            TempData["ToastrMessage"] = "Failed to delete category.";
            TempData["ToastrType"] = "error";
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

    [HttpPost]
    public async Task<IActionResult> AddItem(MenuItemViewModel model)
    {
        if (ModelState.IsValid)
        {
            var token = Request.Cookies["Token"];
            var (email, id, isFirstLogin) = await _tokenDataService.GetEmailFromToken(token!);

            bool isItemExist = _menuService.IsItemExist(model.Name, model.CategoryId);
            if (isItemExist)
            {
                TempData["ToastrMessage"] = "Item Already Exist";
                TempData["ToastrType"] = "error";
                return RedirectToAction("Menu", "Menu");
            }

            // ItemTabViewModel MenuItemTab = _menuService.GetCategoryItem(5, 1, "");

            try
            {
                var response = _menuService.AddNewItem(model, int.Parse(id));
            }
            catch (Exception e)
            {
                TempData["ToastrMessage"] = "Error While Add New Item!";
                TempData["ToastrType"] = "error";
                return RedirectToAction("Menu", "Menu");
            }
            TempData["ToastrMessage"] = "Item Added Successfully";
            TempData["ToastrType"] = "success";
            return RedirectToAction("Menu", "Menu");
        }
        else
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }
            TempData["ToastrMessage"] = "Item Not Added";
            TempData["ToastrType"] = "error";
            // ItemTabViewModel MenuItemTab = _menuService.GetCategoryItem(5, 1, "");
            return RedirectToAction("Menu", "Menu");
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditMenuItem(int itemId)
    {
        var menuItem = _menuService.GetMenuItemById(itemId);
        // return PartialView("_EditItem", menuItem);
        return Json(new { data = menuItem });
    }

    [HttpPost]
    public async Task<IActionResult> EditMenuItem(MenuItemViewModel menuItemViewModel)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_EditItem", menuItemViewModel);
        }

        try
        {
            var AuthToken = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(AuthToken))
                return null;

            var (userEmail, id, isFirstLogin) = await _tokenDataService.GetEmailFromToken(AuthToken);
            if (userEmail == null)
                return null;

            var result = await _menuService.EditItemAsync(menuItemViewModel, int.Parse(id));

            if (result)
            {
                TempData["ToastrMessage"] = "Item edited successfully.";
                TempData["ToastrType"] = "success";
                return Json(new { success = true, redirectUrl = Url.Action("Menu") });
            }
            else
            {
                TempData["ToastrMessage"] = "Failed to update item.";
                TempData["ToastrType"] = "error";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            ModelState.AddModelError("", "Error editing item.");
        }

        return PartialView("_EditItem", menuItemViewModel);
    }

    [HttpPost]
    public async Task<IActionResult>? DeleteMenuItem(int id)
    {
        var token = Request.Cookies["Token"];
        if (token == null)
            return RedirectToAction("Login", "Auth");

        var (email, userId, isFirstLogin) = await _tokenDataService.GetEmailFromToken(token!);
        if (email == null)
            return null!;
        try
        {
            _menuService.DeleteMenuItem(id);
            return Json(new { isSuccess = true, message = "Item Deleted Successfully" });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error While Delete Item" });
        }
    }

    [HttpPost]
    public async Task<IActionResult>? MultiDeleteMenuItem(int[] itemIds)
    {
        var token = Request.Cookies["Token"];
        if (token == null)
            return RedirectToAction("Login", "Auth");
        var (email, userId, isFirstLogin) = await _tokenDataService.GetEmailFromToken(token!);
        if (email == null)
            return null!;
        try
        {
            _menuService.MultiDeleteMenuItem(itemIds);
            return Json(new { isSuccess = true, message = "Items Deleted Successfully" });
        }
        catch (System.Exception)
        {
            return Json(new { isSuccess = false, message = "Error While Delete Item" });
        }
    }

    [HttpGet]
    public IActionResult AddModifier()
    {
        return PartialView("_AddModifier", new MenuModifierViewModel());
    }

    [HttpGet]
    public IActionResult EditModifier()
    {
        return PartialView("_EditModifier", new MenuModifierViewModel());
    }
}