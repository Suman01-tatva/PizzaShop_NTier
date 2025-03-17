using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModels;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Implementations;

public class MenuCategoryRepository : IMenuCategoryRepository
{
    private readonly PizzashopContext _context;

    public MenuCategoryRepository(PizzashopContext context)
    {
        _context = context;
    }

    public async Task<List<MenuCategoryViewModel>> GetAllMenuCategoriesAsync()
    {
        return await _context.MenuCategories
            .Where(c => c.IsDeleted == false)
            .Select(c => new MenuCategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToListAsync();
    }

    public bool AddNewCategory(MenuCategory menuCategory)
    {
        try
        {
            _context.MenuCategories.Add(menuCategory);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<bool> UpdateCategoryBy(MenuCategory menuCategory)
    {
        _context.Set<MenuCategory>().Update(menuCategory);
        return await _context.SaveChangesAsync() > 0;

    }

    public async Task<MenuCategory> GetCategoryByIdAsync(int id)
    {
        return await _context.MenuCategories.FirstOrDefaultAsync(mc => mc.Id == id);
    }
    public bool DeleteCategory(int id)
    {
        var category = _context.MenuCategories.FirstOrDefault(c => c.Id == id);
        if (category != null)
        {
            category.IsDeleted = true;
            _context.SaveChanges();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool GetCategoryByName(string name)
    {
        var category = _context.MenuCategories.FirstOrDefault(c => c.Name == name);
        if (category != null)
        {
            return true;
        }
        return false;
    }
}