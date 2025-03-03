using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.Data;

namespace PizzaShop.Service.Implementations;

public class MenuService : IMenuService
{
    private readonly IMenuCategoryRepository _menuCategoryRepository;
    private readonly IMenuItemsRepository _menuItemRepository;

    public MenuService(IMenuCategoryRepository menuCategoryRepository, IMenuItemsRepository menuItemsRepository)
    {
        _menuCategoryRepository = menuCategoryRepository;
        _menuItemRepository = menuItemsRepository;
    }

    public async Task<List<MenuCategoryViewModel>> GetAllMenuCategoriesAsync()
    {
        return await _menuCategoryRepository.GetAllMenuCategoriesAsync();
    }

    public async Task<List<MenuItemViewModel>> GetItemsByCategory(int categoryId)
    {
        return await _menuItemRepository.GetItemsByCategory(categoryId);
    }
}
