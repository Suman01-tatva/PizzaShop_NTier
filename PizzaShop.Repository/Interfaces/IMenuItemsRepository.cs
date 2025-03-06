using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;

namespace PizzaShop.Repository.Interfaces;

public interface IMenuItemsRepository
{
    Task<List<MenuItem>> GetItemsByCategory(int categoryId, int pageSize, int pageIndex, string? searchString);

    public int GetItemsCountByCId(int cId);

}
