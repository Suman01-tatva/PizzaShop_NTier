using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class MenuModifierController : Controller
{
    private readonly IMenuModifierService _menuModifierService;

    public MenuModifierController(IMenuModifierService menuModifierService)
    {
        _menuModifierService = menuModifierService;
    }

    // [HttpPost]
    // public IActionResult AddNewModifier(string Name, string Description)
    // {
    //     // if (ModelState.IsValid)
    //     // {
    //     //     bool result = _menuService.AddNewCategory(Name, Description);
    //     //     if (result)
    //     //     {
    //     //         return Json(new { success = true, message = "New Category Added" });
    //     //         // return View("Menu", "Menu");
    //     //     }
    //     //     else
    //     //     {
    //     //         return Json(new { success = false, message = "An error occurred while adding the category." });
    //     //     }
    //     // }

    // }

    // [HttpGet]
    // public async Task<IActionResult> GetModifiersByModifierGroup(int modifierGroupId)
    // {
    //     List<MenuModifierViewModel> filteredModifiers = await _menuModifierService.GetModifiersByModifierGroup(modifierGroupId);
    //     return PartialView("_Modifier", filteredModifiers);
    // }

    // [HttpGet]
    // public async Task<JsonResult> GetAllModifierGroups()
    // {
    //     var modifiers = await _menuModifierService.GetAllMenuModifierGroupAsync();
    //     return Json(modifiers);
    // }
}
