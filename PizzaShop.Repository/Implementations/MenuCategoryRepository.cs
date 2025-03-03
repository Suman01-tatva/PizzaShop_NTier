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
}