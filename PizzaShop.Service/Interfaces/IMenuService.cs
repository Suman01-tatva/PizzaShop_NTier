using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IMenuService
{
    Task<List<MenuCategoryViewModel>> GetAllMenuCategoriesAsync();

    Task<List<MenuItemViewModel>> GetItemsByCategory(int categoryId);

    public bool AddNewCategory(string Name, string Description);

    public Task<MenuCategoryViewModel> GetCategoryDetailById(int id);
    public Task<bool> EditCategory(MenuCategoryViewModel model, int categoryId);

    public Task<ItemTabViewModel> GetItemTabDetails(int categoryId);

    public bool SoftDeleteCategory(int id);
}
