using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Repository.Interfaces;

public interface IMenuItemsRepository
{
    Task<List<MenuItem>> GetItemsByCategory(int categoryId, int pageSize, int pageIndex, string? searchString);

    public int GetItemsCountByCId(int cId);

    public bool AddNewItem(MenuItem model);

    public bool IsItemExist(string name, int catId);

    public MenuItem GetMenuItemById(int id);

    public void EditMenuItem(MenuItemViewModel model);

    public void DeleteMenuItem(int id);
}
