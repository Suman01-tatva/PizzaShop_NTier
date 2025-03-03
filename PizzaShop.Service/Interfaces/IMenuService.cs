using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IMenuService
{
    Task<List<MenuCategoryViewModel>> GetAllMenuCategoriesAsync();

    Task<List<MenuItemViewModel>> GetItemsByCategory(int categoryId);
}
