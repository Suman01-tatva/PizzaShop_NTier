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
        var itmes = _context.MenuItems.Where(c => c.CategoryId == categoryId && c.IsDeleted == false);

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

    public bool AddNewItem(MenuItem model)
    {
        try
        {
            _context.MenuItems.Add(model);
            _context.SaveChanges();
            return true;
        }
        catch (Exception Error)
        {
            Console.WriteLine(Error);
            return false;
        }
    }

    public bool IsItemExist(string name, int catId)
    {
        var item = _context.MenuItems.Where(i => i.Name == name && i.CategoryId == catId && i.IsDeleted == false);
        if (item != null)
            return true;
        return false;
    }

    public MenuItem GetMenuItemById(int id)
    {
        var item = _context.MenuItems.FirstOrDefault(i => i.Id == id);
        return item!;
    }

    public void EditMenuItem(MenuItemViewModel model)
    {
        var menuItem = _context.MenuItems.FirstOrDefault(i => i.Id == model.Id);

        _context.SaveChanges();
    }

    public void DeleteMenuItem(int id)
    {
        var menuItem = _context.MenuItems.FirstOrDefault(i => i.Id == id);
        menuItem!.IsDeleted = true;
        _context.SaveChanges();
    }
}
