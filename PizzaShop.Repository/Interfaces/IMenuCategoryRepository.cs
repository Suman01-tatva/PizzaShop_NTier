using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Repository.Interfaces;

public interface IMenuCategoryRepository
{
    Task<List<MenuCategoryViewModel>> GetAllMenuCategoriesAsync();
    public bool AddNewCategory(string Name, string Description);

    public Task<bool> UpdateCategoryBy(MenuCategory menuCategory);

    public Task<MenuCategory> GetCategoryByIdAsync(int id);

    public bool DeleteCategory(int id);
}