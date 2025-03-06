using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Implementations;

public class MenuItemRepository : IMenuItemsRepository
{
    private readonly PizzashopContext _context;

    public MenuItemRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<MenuItem>> GetItemsByCategory(int categoryId, int pageSize, int pageIndex, string? searchString)
    {
        var itmes = _context.MenuItems.Where(c => c.CategoryId == categoryId);

        if (!string.IsNullOrEmpty(searchString))
        {
            itmes = itmes.Where(i => i.Name.ToLower().Contains(searchString.ToLower()));
        }

        var itemList = itmes.OrderBy(u => u.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return itemList;
    }

    public int GetItemsCountByCId(int cId)
    {
        int count = _context.MenuItems.Where(i => i.CategoryId == cId).Count();
        return count;
    }
}
