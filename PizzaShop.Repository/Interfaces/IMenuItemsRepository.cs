using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Repository.Interfaces;

public interface IMenuItemsRepository
{
    Task<List<MenuItemViewModel>> GetItemsByCategory(int categoryId);
}
