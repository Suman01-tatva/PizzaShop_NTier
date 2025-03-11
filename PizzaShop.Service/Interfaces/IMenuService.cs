using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IMenuService
{
    Task<List<MenuCategoryViewModel>> GetAllMenuCategoriesAsync();

    Task<List<MenuItemViewModel>> GetItemsByCategory(int categoryId, int pageSize, int pageIndex, string? searchString);

    Task<bool> AddNewCategory(string category, MenuCategoryViewModel model);

    public Task<MenuCategoryViewModel> GetCategoryDetailById(int id);
    public Task<bool> EditCategory(MenuCategoryViewModel model, int categoryId);

    public Task<ItemTabViewModel> GetItemTabDetails(int categoryId, int pageSize, int pageIndex, string? searchString);

    public bool SoftDeleteCategory(int id);

    public int GetItemsCountByCId(int cId);

    public bool FindCategoryByName(string name);

    public List<Unit> GetAllUnits();
}
