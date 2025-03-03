using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Repository.Interfaces;

public interface IMenuCategoryRepository
{
    Task<List<MenuCategoryViewModel>> GetAllMenuCategoriesAsync();
}