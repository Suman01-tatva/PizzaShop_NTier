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
            .Select(c => new MenuCategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToListAsync();
    }

    public bool AddNewCategory(MenuCategoryViewModel model)
    {
        try
        {
            MenuCategory menuCategory = new MenuCategory
            {
                Name = model.Name,
                Description = model.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = model.CreatedBy
            };

            _context.MenuCategories.Add(menuCategory);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception
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
}