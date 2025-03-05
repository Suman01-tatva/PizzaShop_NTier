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

    public async Task<List<MenuItemViewModel>> GetItemsByCategory(int categoryId)
    {
        var itmes = await _context.MenuItems
            .Select(c => new MenuItemViewModel
            {
                Id = c.Id,
                UnitId = c.UnitId,
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                Type = c.Type,
                Rate = c.Rate,
                Quantity = c.Quantity,
                IsAvailable = c.IsAvailable,
                ShortCode = c.ShortCode,
            }).Where(c => c.CategoryId == categoryId).ToListAsync();

        return itmes;
    }
}
