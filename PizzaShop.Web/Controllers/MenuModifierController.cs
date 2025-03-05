using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class MenuModifierController
{
    private readonly IMenuModifierService _menuModifierService;

    public MenuModifierController(IMenuModifierService menuModifierService)
    {
        _menuModifierService = menuModifierService;
    }

    // [HttpGet]
    // public async Task<IActionResult> Mo(int? categoryId)
    // {
    //     var modifierGroups = await _menuModifierService.GetAllMenuModifierGroupAsync();

    //     var validCategoryId = categoryId ?? categories.First().Id;
    //     var itemTabDetails = await _menuService.GetItemTabDetails(validCategoryId);

    //     var model = new MenuViewModel
    //     {
    //         ItemTab = itemTabDetails
    //     };

    //     return View(model);
    // }

}
